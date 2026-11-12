using System;
using System.Drawing;
using static smartFactoryApp.Models.Parameters;

namespace smartFactoryApp.Models
{
    internal class Robot : Beweger
    {
        // Die 8 Richtungen im Uhrzeigersinn, beginnend oben:
        // Index 0 = 0°, Index 1 = 45°, Index 2 = 90°, ...
        private static readonly Direction[] blickrichtungen =
        {
            Direction.up,
            Direction.upRight,
            Direction.right,
            Direction.downRight,
            Direction.down,
            Direction.downLeft,
            Direction.left,
            Direction.upLeft
        };

        // Blickrichtung in Grad (0 = oben). Nur der Roboter selbst darf sie ändern.
        public double Angle { get; private set; }

        public Robot(Point location, Rectangle borders) : base(location, borders)
        {
            Step = Parameters.robotStep;   // Der Roboter fährt 5 Pixel pro Schritt
        }

        // ===================== DREHEN =====================

        public void RotateLeft()
        {
            Rotate(-5);
        }

        public void RotateRight()
        {
            Rotate(5);
        }

        private void Rotate(double grad)
        {
            // Winkel ändern und im Bereich 0 bis 359 halten
            Angle = (Angle + grad + 360) % 360;

            // Ein gehaltener Brick folgt der Hand
            DockobjektNachziehen();
        }

        // ===================== BEWEGEN =====================

        // Fährt relativ zur Blickrichtung: "hoch" heißt "dorthin, wohin ich schaue"
        public void BewegenRelativ(Direction direction)
        {
            Direction echteRichtung = RichtungBerechnen(direction);

            base.Bewegen(echteRichtung);
        }

        private Direction RichtungBerechnen(Direction direction)
        {
            // Wie viele 45°-Schritte ist der Roboter gedreht?
            int winkelSchritt = (int)Math.Round(Angle / 45.0) % 8;

            // Wie viele 45°-Schritte ist die gedrückte Taste von "oben" entfernt?
            int relativSchritt = Math.Max(0, Array.IndexOf(blickrichtungen, direction));

            // Beides zusammenzählen (nach 8 Schritten beginnt es wieder bei 0)
            int index = (winkelSchritt + relativSchritt) % 8;

            return blickrichtungen[index];
        }
    }
}