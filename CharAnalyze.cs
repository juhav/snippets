        public static bool IsAllowedChar(char c, out char output)
        {
            var category = Char.GetUnicodeCategory(c);

            output = c;

            switch (category)
            {
                case UnicodeCategory.UppercaseLetter: return true;
                case UnicodeCategory.LowercaseLetter: return true;
                case UnicodeCategory.TitlecaseLetter: return true;
                case UnicodeCategory.ModifierLetter: return true;
                case UnicodeCategory.OtherLetter: return false;
                case UnicodeCategory.NonSpacingMark: return true;
                case UnicodeCategory.SpacingCombiningMark: return true;
                case UnicodeCategory.EnclosingMark: return true;
                case UnicodeCategory.DecimalDigitNumber: return true;
                case UnicodeCategory.LetterNumber: return true;
                case UnicodeCategory.OtherNumber: return true;
                case UnicodeCategory.SpaceSeparator:
                    output = '\u0020';
                    return true;
                case UnicodeCategory.LineSeparator: return false;
                case UnicodeCategory.ParagraphSeparator: return false;
                case UnicodeCategory.Control: return false;
                case UnicodeCategory.Format: return true;
                case UnicodeCategory.Surrogate: return true;
                case UnicodeCategory.PrivateUse: return false;
                case UnicodeCategory.ConnectorPunctuation: return true;
                case UnicodeCategory.DashPunctuation: return true;
                case UnicodeCategory.OpenPunctuation: return true;
                case UnicodeCategory.ClosePunctuation: return true;
                case UnicodeCategory.InitialQuotePunctuation: return true;
                case UnicodeCategory.FinalQuotePunctuation: return true;
                case UnicodeCategory.OtherPunctuation: return true;
                case UnicodeCategory.MathSymbol: return true;
                case UnicodeCategory.CurrencySymbol: return true;
                case UnicodeCategory.ModifierSymbol: return false;
                case UnicodeCategory.OtherSymbol: return false;
                case UnicodeCategory.OtherNotAssigned: return false;
            }

            return false;
        }

private static void CreateCharTable()
        {
            var sb = new StringBuilder();

            sb.AppendLine("<!doctype html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset=\"UTF-8\"/>");
            sb.AppendLine("<style>table { border-collapse: collapse; } table, th, td { border: 1px solid black; } </style>");
            sb.AppendLine("</head>");

            sb.AppendLine("<body>");
            
            sb.AppendLine("<table>");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th>#</th>");
            sb.AppendLine("<th>Char</th>");
            sb.AppendLine("<th>IsControl</th>");
            sb.AppendLine("<th>IsLetterOrDigit</th>");
            sb.AppendLine("<th>UTF-8</th>");
            sb.AppendLine("<th>Unicode Category</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");

            sb.AppendLine("<tbody>");
            for (int i=0; i<=ushort.MaxValue; i++)
            {
                Char c = (char)i;

                sb.Append("<tr>");

                sb.Append("<td>");
                sb.Append(i);
                sb.Append("</td>");

                sb.Append("<td>");
                sb.AppendFormat("&#{0};", i);
                sb.Append("</td>");

                sb.Append("<td>");
                sb.Append(Char.IsControl(c) ? "X" : "");
                sb.Append("</td>");

                sb.Append("<td>");
                sb.Append(Char.IsLetterOrDigit(c) ? "X" : "");
                sb.Append("</td>");

                sb.Append("<td>");
                var s = new string(new Char[] { c } );

                var bytes = Encoding.UTF8.GetBytes(s);
                
                for (int j=0; j<bytes.Length; j++)
                {
                    if (j>0)
                    {
                        sb.Append("-");
                    }
                    sb.Append((int)bytes[j]);
                }
                sb.Append("</td>");

                sb.Append("<td>");
                sb.Append(Char.GetUnicodeCategory(c).ToString());
                sb.Append("</td>");
                    
                sb.Append("</tr>");
            }

            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            System.IO.File.WriteAllText("c:\\temp\\chars.html", sb.ToString());
        }
        
        
        public static string SanitizeFolderName(string name)
{
    string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
    var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
    return r.Replace(name, "");
}
