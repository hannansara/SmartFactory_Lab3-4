using System.Collections.Generic;
using System.Drawing;

namespace smartFactoryApp.Models
{
    internal class ProductionLine
    {
        private Point location;

        private List<LegoBrick> bricks = new List<LegoBrick>();

        private Parameters.Direction side;

        public Point Location { get => location; }
        public Parameters.LegoColor Color { get; }
        public IReadOnlyList<LegoBrick> Bricks => bricks;

        public ProductionLine(
            Point location,
            Parameters.LegoColor color,
            Parameters.Direction side)
        {
            this.location = location;
            Color = color;
            this.side = side;

            CreateBricks();
        }

        private void CreateBricks()
        {
            int count = Parameters.bricksPerLine;
            int spacing = ViewParameters.ProductionLineWidth / count;

            for (int i = 0; i < count; i++)
            {
                Point brickLocation;

                // Produktionslinie oben oder unten
                if (side == Parameters.Direction.up ||
                    side == Parameters.Direction.down)
                {
                    int x = location.X+ i * spacing+ spacing / 2- 10;
                    int y;
                    // Obere Linie:
                    // Bricks liegen an der unteren Kante
                    if (side == Parameters.Direction.up)
                    {
                        y = location.Y+ ViewParameters.ProductionLineHeight- 5;
                    }
                    // Untere Linie:
                    // Bricks liegen an der oberen Kante
                    else
                    {
                        y = location.Y + 5;
                    }

                    brickLocation = new Point(x, y);
                }

                // Produktionslinie links oder rechts
                else
                {
                    int y = location.Y + i * spacing + spacing / 2- 10;

                    int x;

                    // Linke Linie:
                    // Bricks liegen an der rechten Kante
                    if (side == Parameters.Direction.left)
                    {
                        x = location.X + ViewParameters.ProductionLineHeight- 5;
                    }
                    // Rechte Linie:
                    // Bricks liegen an der linken Kante
                    else
                    {
                        x = location.X + 5;
                    }

                    brickLocation = new Point(x, y);
                }

                bricks.Add(new LegoBrick(brickLocation, Color));
            }
        }

        private int tickCount = 0;
        private const int stepsPerDirection = 10;

        public void Bewegen()
        {
            bool outward = tickCount < stepsPerDirection;

            tickCount =
                (tickCount + 1) % (2 * stepsPerDirection);

            for (int i = 0; i < bricks.Count; i++)
            {
                // Oben und unten:
                // horizontal bewegen
                if (side == Parameters.Direction.up ||
                    side == Parameters.Direction.down)
                {
                    if (outward)
                    {
                        bricks[i].Bewegen(Parameters.Direction.right);
                    }
                    else
                    {
                        bricks[i].Bewegen(Parameters.Direction.left);
                    }
                }

                // Links und rechts:
                // vertikal bewegen
                else
                {
                    if (outward)
                    {
                        bricks[i].Bewegen( Parameters.Direction.down );
                    }
                    else
                    {
                        bricks[i].Bewegen(Parameters.Direction.up);
                    }
                }
            }
        }
    }
}