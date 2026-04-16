namespace AdventurePdfForge;

public class Adventure
{
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public string IntroTitle { get; set; } = "";
    public List<string> IntroText { get; set; } = new();
    public List<string> SummaryBullets { get; set; } = new();
    public List<string> ClueBullets { get; set; } = new();
    public string IntroImage { get; set; } = "";
    public string BackgroundImage { get; set; } = "";
    public string FrameImage { get; set; } = "";
}
