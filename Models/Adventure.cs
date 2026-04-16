namespace AdventurePdfForge;

public class Adventure
{
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public string IntroTitle { get; set; } = "";
    public List<string> IntroText { get; set; } = [];
    public List<string> SummaryBullets { get; set; } = [];
    public List<string> ClueBullets { get; set; } = [];

    // Pagina 3: Hook + Twist
    public string Hook { get; set; } = "";
    public string Twist { get; set; } = "";

    // Pagina 4: Indizi dettagliati
    public List<ClueDetail> CluesDetailed { get; set; } = [];

    // Pagina 5: Struttura scena
    public SceneStructureData SceneStructure { get; set; } = new();

    // Pagina 6: Fail forward
    public FailForwardData FailForward { get; set; } = new();
    public string FooterNote { get; set; } = "";
}

public class ClueDetail
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Interpretation { get; set; } = "";
    public string Risk { get; set; } = "";
}

public class SceneStructureData
{
    public string Entry { get; set; } = "";
    public string Distortion { get; set; } = "";
    public string Revelation { get; set; } = "";
    public string Climax { get; set; } = "";
}

public class FailForwardData
{
    public string Failure { get; set; } = "";
    public string Consequence { get; set; } = "";
    public string Escalation { get; set; } = "";
}
