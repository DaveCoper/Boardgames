namespace Boardgames.NinthPlanet.Models
{
    public enum TaskCardModifier { 
        First,             // card must be taken first  ( 1 symbol ) 
        Second,            // card must be taken second ( 2 symbol ) 
        Third,             // card must be taken third  ( 3 symbol )
        Fourth,            // card must be taken Fourth ( 4 symbol ) 
        Fifth,             // card must be taken Fifth  ( 5 symbol )
        Last,              // card must be taken Last   ( Ω symbol )

        CriticalPriority,  // card must be taken before card with high priority     ( > symbol )
        HighPriority,      // card must be taken before card with medium priority   ( >> symbol )
        MediumPriority,    // card must be taken before card with low priority      ( >>> symbol )
        LowPriority,       // card must be taken last card with priority            ( >>>> symbol )
    };
}
