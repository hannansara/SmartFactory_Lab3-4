using System;
using System.Collections.Generic;
using Avalonia.Threading;
using smartFactoryApp.Models;
using static smartFactoryApp.Models.Parameters;

namespace smartFactoryApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    // ===================== DATEN =====================

    private readonly Factory factory = new Factory();

    private readonly DispatcherTimer timer;        // bewegt die LegoBricks
    private readonly DispatcherTimer robotTimer;   // wiederholt die Roboter-Aktion (Move-Modus)

    private enum RobotAktion { Bewegen, LinksDrehen, RechtsDrehen }

    private RobotAktion currentRobotAktion = RobotAktion.Bewegen;
    private Direction currentRobotDirection;

    private bool moveMode = false;   // false = Step-Modus, true = Move-Modus

    public List<ProductionLineViewModel> ProductionLines { get; } = new List<ProductionLineViewModel>();
    public List<LegoBrickViewModel> Bricks { get; } = new List<LegoBrickViewModel>();

    // ===================== WERTE FÜR DIE OBERFLÄCHE =====================

    public double RobotX => factory.RobotX;
    public double RobotY => factory.RobotY;
    public double RobotAngle => factory.RobotAngle;

    private string undockMessage = "";

    public string UndockMessage
    {
        get { return undockMessage; }
        set
        {
            undockMessage = value;
            OnPropertyChanged(nameof(UndockMessage));
        }
    }

    // ===================== KONSTRUKTOR =====================

    public MainViewModel()
    {
        foreach (var line in factory.ProductionLines)
        {
            ProductionLines.Add(new ProductionLineViewModel(line));
        }

        foreach (var brick in factory.Bricks)
        {
            Bricks.Add(new LegoBrickViewModel(brick));
        }

        timer = NeuerTimer(Timer_Tick);
        robotTimer = NeuerTimer(RobotTimer_Tick);
    }

    private static DispatcherTimer NeuerTimer(EventHandler tick)
    {
        var neuerTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(timerInterval)
        };

        neuerTimer.Tick += tick;

        return neuerTimer;
    }

    // ===================== ROBOTER: 8 RICHTUNGEN =====================

    public void MoveLeft()      => RobotSteuern(Direction.left);
    public void MoveRight()     => RobotSteuern(Direction.right);
    public void MoveUp()        => RobotSteuern(Direction.up);
    public void MoveDown()      => RobotSteuern(Direction.down);
    public void MoveUpLeft()    => RobotSteuern(Direction.upLeft);
    public void MoveUpRight()   => RobotSteuern(Direction.upRight);
    public void MoveDownLeft()  => RobotSteuern(Direction.downLeft);
    public void MoveDownRight() => RobotSteuern(Direction.downRight);

    public void RotateLeft()  => RobotStarten(RobotAktion.LinksDrehen);
    public void RotateRight() => RobotStarten(RobotAktion.RechtsDrehen);

    private void RobotSteuern(Direction direction)
    {
        RobotStarten(RobotAktion.Bewegen, direction);
    }

    // Step-Modus: Aktion einmal ausführen. Move-Modus: Timer wiederholt sie.
    private void RobotStarten(RobotAktion aktion, Direction direction = Direction.up)
    {
        currentRobotAktion = aktion;
        currentRobotDirection = direction;   // wird nur beim Bewegen benutzt

        if (moveMode)
        {
            robotTimer.Start();
        }
        else
        {
            AktionAusfuehren();
        }
    }

    // Führt die aktuelle Aktion genau einmal aus und aktualisiert die Anzeige
    private void AktionAusfuehren()
    {
        switch (currentRobotAktion)
        {
            case RobotAktion.Bewegen:
                factory.RobotBewegen(currentRobotDirection);
                break;

            case RobotAktion.LinksDrehen:
                factory.RobotRotateLeft();
                break;

            case RobotAktion.RechtsDrehen:
                factory.RobotRotateRight();
                break;
        }

        NotifyRobotPositionChanged();
        NotifyLegoBricksPositionChanged();
    }

    // ===================== MODI =====================

    public void SetStepMode()
    {
        moveMode = false;
        robotTimer.Stop();
    }

    public void SetMoveMode()
    {
        moveMode = true;
    }

    public void StopRobot()
    {
        robotTimer.Stop();
    }

    // ===================== TIMER =====================

    public void StartTimer() => timer.Start();
    public void StopTimer()  => timer.Stop();

    private void RobotTimer_Tick(object? sender, EventArgs e)
    {
        AktionAusfuehren();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        factory.LegoBricksBewegen();
        NotifyLegoBricksPositionChanged();
    }

    // ===================== UNDOCK =====================

    public void Undock()
    {
        UndockResult result = factory.Undock();

        UndockMessage = result switch
        {
            UndockResult.NoBrick           => "No brick available to undock.",
            UndockResult.NotOnDeliveryLine => "Undocking is only possible on the Delivery Line.",
            UndockResult.SuccessfulGood    => "Undocking successful. Brick is being delivered to the customer.",
            UndockResult.SuccessfulBad     => "Brick is defective and is being sent to the Recycling Line.",
            _                              => ""
        };

        NotifyRobotPositionChanged();
        NotifyLegoBricksPositionChanged();
    }

    // ===================== ANZEIGE AKTUALISIEREN =====================

    private void NotifyRobotPositionChanged()
    {
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(RobotAngle));
    }

    private void NotifyLegoBricksPositionChanged()
    {
        foreach (var brick in Bricks)
        {
            brick.Refresh();
        }
    }
}