using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AvaloniaBoardSpike;

public class PromptCardModel : INotifyPropertyChanged
{
    public PromptCardModel(string title, string icon, string body, string[] tags, bool isFavorite, int versions, string accent)
    {
        Title = title;
        Icon = icon;
        Body = body;
        Tags = tags;
        IsFavorite = isFavorite;
        Versions = versions;
        Accent = accent;
    }

    public string Title { get; }
    public string Icon { get; }
    public string Body { get; }
    public string[] Tags { get; }
    public int Versions { get; }
    public string Accent { get; }

    private bool _isFavorite;
    public bool IsFavorite
    {
        get => _isFavorite;
        set { _isFavorite = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class BoardColumn
{
    public BoardColumn(string title)
    {
        Title = title;
        Cards = new ObservableCollection<PromptCardModel>();
    }

    public string Title { get; }
    public ObservableCollection<PromptCardModel> Cards { get; }
}

public static class MockData
{
    // Column names mirror the current Uno build (MainPage.xaml:
    // Ide Awal / Dikembangkan / Siap Digunakan).
    public static BoardColumn IdeAwal() => new BoardColumn("Ide Awal")
    {
        Cards =
        {
            new PromptCardModel("Blog Post Outline", "💡",
                "You are a senior content strategist. Given a topic and target audience, produce a 7-section outline. For each section: H2 heading, 3 key questions it answers, and a suggested example. Tone: practical, no fluff.",
                new[] { "writing", "seo" }, true, 3, "#22C55E"),
            new PromptCardModel("Meeting Notes → Actions", "📝",
                "Summarize the transcript into: decisions made, action items with owners, and open questions. Flag anything ambiguous instead of guessing.",
                new[] { "productivity" }, false, 1, "#22C55E"),
            new PromptCardModel("Naming Brainstorm", "🧠",
                "Generate 20 names for a developer tool that organizes AI prompts. Constraints: 2 syllables max, no 'AI' suffix. Group by vibe: playful / serious / abstract.",
                new[] { "creative" }, false, 2, "#22C55E"),
        }
    };

    public static BoardColumn Dikembangkan() => new BoardColumn("Dikembangkan")
    {
        Cards =
        {
            new PromptCardModel("Code Review Assistant", "🔨",
                "Review the following diff. Categorize findings as: bug, perf, style, or security. Be specific about line numbers. Skip nitpicks unless asked.",
                new[] { "coding", "review" }, true, 4, "#FACC15"),
            new PromptCardModel("SQL Explainer", "🗄️",
                "Explain this query step by step as if to a junior dev. Call out missing indexes and potential full scans. Then rewrite it with CTEs for readability.",
                new[] { "coding", "sql" }, false, 2, "#FACC15"),
            new PromptCardModel("Unit Test Generator", "🧪",
                "Given this C# function, write xUnit tests covering: happy path, edge cases, and failure modes. Use FluentAssertions. Name tests Method_Scenario_Expectation.",
                new[] { "coding", "tdd" }, false, 3, "#FACC15"),
        }
    };

    public static BoardColumn SiapDigunakan() => new BoardColumn("Siap Digunakan")
    {
        Cards =
        {
            new PromptCardModel("Daily Standup Summary", "🚀",
                "Turn these bullet points into a crisp 60-second standup update: yesterday, today, blockers. Maximum 5 sentences.",
                new[] { "productivity" }, true, 5, "#3B82F6"),
            new PromptCardModel("Regex Builder", "🔍",
                "Build a regex for the requirement below. Explain each token on its own line. Provide 3 test strings that match and 3 that must not.",
                new[] { "coding", "tools" }, false, 2, "#3B82F6"),
            new PromptCardModel("Email Polisher", "✉️",
                "Rewrite this email to be 40% shorter while keeping it warm. Keep my sign-off. Return only the rewritten email.",
                new[] { "writing" }, false, 1, "#3B82F6"),
        }
    };
}
