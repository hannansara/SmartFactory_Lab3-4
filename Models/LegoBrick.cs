using System.Drawing;

namespace smartFactoryApp.Models
{
    public class LegoBrick : Beweger
    {
        // ===================== DATEN =====================

        public Parameters.LegoColor Color { get; }                          // Farbe (ändert sich nie)
        public bool Gedockt { get; set; }                                   // true = Roboter hält ihn
        public double Angle { get; set; }                                   // Drehwinkel
        public Parameters.Quality Quality { get; set; } = Parameters.Quality.unknown;
        public bool HorizontalNachUndock { get; set; }                      // true = fährt automatisch

        private Point startPosition;                 // hier startet der Brick
        private bool aufRecyclingBand = false;       // true = bleibt auf dem Band liegen

        // Zählt, welcher Platz auf dem Recycling-Band als Nächstes dran ist
        private static int naechsterRecyclingPlatz = 0;

        public LegoBrick(Point location, Parameters.LegoColor color)
            : base(location, ViewParameters.BrickBorders)
        {
            Color = color;
            startPosition = location;
        }

        // ===================== BEWEGUNG =====================

        public override void Bewegen(Parameters.Direction dir)
        {
            // Gedockt: der Roboter trägt ihn, er bewegt sich nicht selbst
            if (Gedockt)
            {
                return;
            }

            // Auf dem Recycling-Band: bleibt für immer liegen
            if (aufRecyclingBand)
            {
                return;
            }

            // Nach dem Ablegen: fährt automatisch (gut = rechts, schlecht = links)
            if (HorizontalNachUndock)
            {
                AutomatischBewegen();
                return;
            }

            // Normalfall: auf der Produktionslinie hin und her fahren
            base.Bewegen(dir);
        }

        private void AutomatischBewegen()
        {
            if (Quality == Parameters.Quality.good)
            {
                // Gut: nach rechts zum Kunden
                base.Bewegen(Parameters.Direction.right);

                if (Location.X >= ViewParameters.FactoryZonesRect.Right)
                {
                    ZurStartPosition();
                }
            }
            else if (Quality == Parameters.Quality.bad)
            {
                // Schlecht: nach links zum Recycling
                base.Bewegen(Parameters.Direction.left);

                if (Location.X <= ViewParameters.FactoryZonesRect.Left)
                {
                    AufRecyclingBandLegen();
                }
            }
        }

        // ===================== HILFSMETHODEN =====================

        // Setzt Qualität und Bewegungs-Zustand zurück
        private void Zuruecksetzen()
        {
            Quality = Parameters.Quality.unknown;
            HorizontalNachUndock = false;
            Gedockt = false;
        }

        // Guter Brick: zurück an den Startplatz auf seiner Produktionslinie
        private void ZurStartPosition()
        {
            Location = startPosition;
            Angle = 0;
            aufRecyclingBand = false;
            Zuruecksetzen();
        }

        // Schlechter Brick: auf den nächsten freien Platz auf dem Recycling-Band
        private void AufRecyclingBandLegen()
        {
            Location = RecyclingPosition();

            // Rot und Gelb liegen schon waagerecht, Blau und Grün werden gedreht
            if (Color == Parameters.LegoColor.red || Color == Parameters.LegoColor.yellow)
            {
                Angle = 0;
            }
            else
            {
                Angle = 90;
            }

            aufRecyclingBand = true;
            Zuruecksetzen();
        }

        // Berechnet die Position des nächsten Platzes (es gibt 4 Plätze)
        private static Point RecyclingPosition()
        {
            const int anzahlPlaetze = 4;

            Rectangle band = ViewParameters.MultiColorRect;
            int abstand = band.Width / anzahlPlaetze;
            int platz = naechsterRecyclingPlatz % anzahlPlaetze;

            naechsterRecyclingPlatz++;

            int x = band.Left + abstand / 2 + platz * abstand;
            int y = band.Top + 5;

            return new Point(x, y);
        }

        public override string ToString()
        {
            return $"{Color}";
        }
    }
}