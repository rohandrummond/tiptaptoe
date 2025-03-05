using System.Text.Json.Serialization;

namespace TipTapToe.Models
{

    public class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public required List<ResponseContent> Candidates { get; set; }
    }

    public class ResponseContent
    {
        [JsonPropertyName("content")]
        public required ResponseParts Content { get; set; }
    }

    public class ResponseParts
    {
        [JsonPropertyName("parts")]
        public required List<ResponsePart> Parts { get; set; }
    }

    public class ResponsePart
    {
        [JsonPropertyName("text")]
        public required string Part { get; set; }
    }

    public class ResponseText
    {
        [JsonPropertyName("practiceText")]
        public required string PracticeText { get; set; }

        [JsonPropertyName("reasoning")]
        public required string Reasoning { get; set; }
    }

}

// {
//   "candidates": [
//     {
//       "content": {
//         "parts": [
//           {
//             "text": "[{\"practiceText\": \"name = \\\"Enter your name here\\\";\\nprint(\\\"Hello, \\\" + name + \\\", how are you today?\\\");\\n\\nprint(\\\"Additional practice:\\\");\\nname = \\\"Jane Doe\\\";\\nprint(name);\\nname = \\\"John Smith\\\";\\nprint(name);\", \"reasoning\": \"This text focuses on improving accuracy with quotes, parentheses, and variable assignments. It also reinforces correct spacing around operators and punctuation. The structure mimics simple print statements frequently encountered in beginner programming.\"}]"
//           }
//         ],
//         "role": "model"
//       },
//       "finishReason": "STOP",
//       "avgLogprobs": -0.33007579014219085
//     }
//   ],
//   "usageMetadata": {
//     "promptTokenCount": 2273,
//     "candidatesTokenCount": 116,
//     "totalTokenCount": 2389,
//     "promptTokensDetails": [
//       {
//         "modality": "TEXT",
//         "tokenCount": 2273
//       }
//     ],
//     "candidatesTokensDetails": [
//       {
//         "modality": "TEXT",
//         "tokenCount": 116
//       }
//     ]
//   },
//   "modelVersion": "gemini-1.5-flash"
// }