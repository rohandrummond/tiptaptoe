# Tip Tap Toe ⌨️

![Tip Tap Toe Screenshots](https://github.com/rohandrummond/tiptaptoe/blob/main/Assets/tip-tap-toe-screenshots.png?raw=true)

**About the project**

Tip Tap Toe is a typing practice console application designed specifically for developers. It uses a custom API integration (due to no C# library at the time) with Google's Gemini model to generate language specific code sequences, and analyses your keystroke patterns to create personalised practice exercises that target your weak points.

**Why I built it**

I wanted to improve my typing speed when programming, but rather than doing this in a browser I wanted something I could launch directly from my CLI. I hadn't built a console app before (which I feel is a write of passage for most developers), so I thought this would be the perfect opportunity.

While I originally started building it for fun, it quickly became a great chance to practice some of the fundamentals in C#, and a crash course in working with structured data. It let me explore making HTTP requests, real time keystroke capture and enforcing structured JSON output from LLMs.

## Key features 💡

**AI generated practice**

- Gemini generates realistic, syntactically valid code sequences based on your chosen language
- Keystroke analysis identifies slow transitions and error patterns
- Each practice round adapts based on your previous performance data

**Real time keystroke tracking**

- Precise timing using a `Stopwatch` instance
- Tracks every keypress including backspaces to understand error patterns
- Logs success/failure for each character against the target sequence

**WPM (words per minute) calculation and benchmarks**

- Calculates words per minute after each completed round
- Displays benchmarks (Beginner to Elite) to track your progress
- Supports continuous practice sessions with fresh sequences each round

**Multi-Language Support**

- Python, C++, Java, C#, and JavaScript
- Generated code uses realistic patterns, variable names, and syntax for each language

## Tech stack ⚙️

**Framework**

- .NET 9 / C#

**AI Model**

- Google Gemini


## Getting Started 🚀

**Prerequisites**

- .NET SDK
- Google AI Studio API key

**Clone the repository**

```bash
git clone https://github.com/rohandrummond/tiptaptoe.git
cd tiptaptoe
```

**Set up environment variables**

Set your Gemini API key as an environment variable:

```bash
# macOS/Linux
export GEMINI_API_KEY=your_api_key_here

# Windows (PowerShell)
$env:GEMINI_API_KEY="your_api_key_here"

# Windows (Command Prompt)
set GEMINI_API_KEY=your_api_key_here
```

**Run the project**

```bash
dotnet run
```

Follow the prompts to select a programming language and start practicing.
