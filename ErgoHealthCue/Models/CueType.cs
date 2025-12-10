namespace ErgoHealthCue.Models;

public enum CueType
{
    // Position changes
    DeskStanding,
    DeskSitting,
    DeskFloor,
    
    // Position-specific stretches and drills
    StandingStretch,
    StandingMobilityDrill,
    SittingStretch,
    SittingMobilityDrill,
    FloorStretch,
    FloorMobilityDrill
}

public enum DeskPosition
{
    Standing,
    Sitting,
    Floor
}
