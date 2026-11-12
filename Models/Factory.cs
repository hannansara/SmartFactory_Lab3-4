using System.Collections.Generic;
using static smartFactoryApp.Models.Parameters;

namespace smartFactoryApp.Models;

public class Factory
{
    // ===================== DATEN =====================

    private readonly Robot robot =
        new Robot(ViewParameters.RobotPosition, ViewParameters.Borders);

    private readonly List<ProductionLine> productionLines = new List<ProductionLine>();
    private readonly List<LegoBrick> bricks = new List<LegoBrick>();

    // ===================== FÜR DIE OBERFLÄCHE =====================

    public double RobotX => robot.Location.X;
    public double RobotY => robot.Location.Y;
    public double RobotAngle => robot.Angle;

    public IReadOnlyList<LegoBrick> Bricks => bricks;
    internal IReadOnlyList<ProductionLine> ProductionLines => productionLines;

    // ===================== AUFBAU DER FABRIK =====================

    public Factory()
    {
        // Vier Produktionslinien (Position, Farbe, Seite)
        productionLines.Add(new ProductionLine(ViewParameters.RedProductionLinePosition, LegoColor.red, Direction.down));
        productionLines.Add(new ProductionLine(ViewParameters.YellowProductionLinePosition, LegoColor.yellow, Direction.down));
        productionLines.Add(new ProductionLine(ViewParameters.BlueProductionLinePosition, LegoColor.blue, Direction.left));
        productionLines.Add(new ProductionLine(ViewParameters.GreenProductionLinePosition, LegoColor.green, Direction.right));

        // Alle Bricks aller Linien in eine gemeinsame Liste legen
        foreach (var productionLine in productionLines)
        {
            bricks.AddRange(productionLine.Bricks);
        }
    }

    // ===================== ROBOTER STEUERN =====================

    public void RobotBewegen(Direction dir)
    {
        robot.BewegenRelativ(dir);
        KontaktPruefen();
    }

    public void RobotRotateLeft()
    {
        robot.RotateLeft();
        KontaktPruefen();
    }

    public void RobotRotateRight()
    {
        robot.RotateRight();
        KontaktPruefen();
    }

    public UndockResult Undock()
    {
        return robot.Undock();
    }

    // ===================== BRICKS BEWEGEN =====================

    public void LegoBricksBewegen()
    {
        foreach (var productionLine in productionLines)
        {
            productionLine.Bewegen();
        }
    }

    // Ist ein Brick nah genug an der Hand? Dann nimmt der Roboter ihn auf.
    private void KontaktPruefen()
    {
        foreach (var brick in bricks)
        {
            robot.Kontakt(brick);
        }
    }
}