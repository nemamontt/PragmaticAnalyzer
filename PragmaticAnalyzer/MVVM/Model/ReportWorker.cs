using PragmaticAnalyzer.Databases;
using PragmaticAnalyzer.MVVM.ViewModel.Main;
using System.ComponentModel;
using System.Reflection;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace PragmaticAnalyzer.MVVM.Model
{
    public class ReportWorker
    {
        public static void CreateReport(DocX doc, Report report)
        {
            doc.MarginTop = 28.35f;
            doc.MarginBottom = 28.35f;
            doc.MarginLeft = 28.35f;
            doc.MarginRight = 28.35f;

            if (report.Specialist is not null)
            {
                Dictionary<string, string> fields = [];

                fields.Add(GetDescription<Specialist>(nameof(report.Specialist.NameOrgan)), report.Specialist.NameOrgan);
                fields.Add(GetDescription<Specialist>(nameof(report.Specialist.NameHighestOrgan)), report.Specialist.NameHighestOrgan);
                fields.Add(GetDescription<Specialist>(nameof(report.Specialist.NameSubordinateOrgan)), report.Specialist.NameSubordinateOrgan);
                fields.Add(GetDescription<Specialist>(nameof(report.Specialist.StatusVulnerability)), report.Specialist.StatusVulnerability);
                fields.Add(GetDescription<Specialist>(nameof(report.Specialist.ActionsTaken)), report.Specialist.ActionsTaken);
                fields.Add(GetDescription<Specialist>(nameof(report.Specialist.NameSoftware)), report.Specialist.NameSoftware);
                fields.Add(GetDescription<Specialist>(nameof(report.Specialist.NameInteractingOrgans)), report.Specialist.NameInteractingOrgansToString);
                fields.Add(GetDescription<Specialist>(nameof(report.Specialist.UsingMeasures)), report.Specialist.UsingMeasuresToString);

                AddTable(doc, fields, "Специалистов по ЗИ");
            }
            if (report.Consequence is not null)
            {
                Dictionary<string, string> fields = [];

                fields.Add(GetDescription<Consequence>(nameof(report.Consequence.Number)), report.Consequence.Number);
                fields.Add(GetDescription<Consequence>(nameof(report.Consequence.Name)), report.Consequence.Name);
                fields.Add(GetDescription<Consequence>(nameof(report.Consequence.Damage)), report.Consequence.Damage);
                fields.Add(GetDescription<Consequence>(nameof(report.Consequence.NameThreatsToString)), report.Consequence.NameThreatsToString);

                AddTable(doc, fields, "Негативные последствия");
            }
            if (report.Technology is not null)
            {
                Dictionary<string, string> fields = [];

                fields.Add(GetDescription<Technology>(nameof(report.Technology.SequenceNumber)), report.Technology.SequenceNumber);
                fields.Add(GetDescription<Technology>(nameof(report.Technology.MethodName)), report.Technology.MethodName);
                fields.Add(GetDescription<Technology>(nameof(report.Technology.Description)), report.Technology.Description);
                fields.Add(GetDescription<Technology>(nameof(report.Technology.Usage)), report.Technology.Usage);
                fields.Add(GetDescription<Technology>(nameof(report.Technology.Scale)), report.Technology.Scale);
                fields.Add(GetDescription<Technology>(nameof(report.Technology.Horizont)), report.Technology.Horizont);
                fields.Add(GetDescription<Technology>(nameof(report.Technology.Level)), report.Technology.Level);
                fields.Add(GetDescription<Technology>(nameof(report.Technology.Necessity)), report.Technology.Necessity);
                fields.Add(GetDescription<Technology>(nameof(report.Technology.Experience)), report.Technology.Experience);
                fields.Add(GetDescription<Technology>(nameof(report.Technology.Сharacteristic)), report.Technology.Сharacteristic);
                fields.Add(GetDescription<Technology>(nameof(report.Technology.Effort)), report.Technology.Effort);

                AddTable(doc, fields, "Технологии оценки риска");
            }
            /*if(report.Violator is not null)
            {

            }*/
            if (report.ProtectionMeasure is not null)
            {
                Dictionary<string, string> fields = [];

                fields.Add(GetDescription<ProtectionMeasure>(nameof(report.ProtectionMeasure.NameGroup)), report.ProtectionMeasure.NameGroup);
                fields.Add(GetDescription<ProtectionMeasure>(nameof(report.ProtectionMeasure.DispayedName)), report.ProtectionMeasure.DispayedName);
                fields.Add(GetDescription<ProtectionMeasure>(nameof(report.ProtectionMeasure.FullName)), report.ProtectionMeasure.FullName);
                fields.Add(GetDescription<ProtectionMeasure>(nameof(report.ProtectionMeasure.SecurityClasses)), report.ProtectionMeasure.SecurityClasses);

                AddTable(doc, fields, "Мер защиты");
            }
            if (report.Threat is not null)
            {
                Dictionary<string, string> fields = [];

                fields.Add(GetDescription<Threat>(nameof(report.Threat.Id)), report.Threat.Id);
                fields.Add(GetDescription<Threat>(nameof(report.Threat.Name)), report.Threat.Name);
                fields.Add(GetDescription<Threat>(nameof(report.Threat.Description)), report.Threat.Description);
                fields.Add(GetDescription<Threat>(nameof(report.Threat.Source)), report.Threat.Source);
                fields.Add(GetDescription<Threat>(nameof(report.Threat.ObjectInfluence)), report.Threat.ObjectInfluence);
                fields.Add(GetDescription<Threat>(nameof(report.Threat.PrivacyViolation)), report.Threat.PrivacyViolation);
                fields.Add(GetDescription<Threat>(nameof(report.Threat.IntegrityViolation)), report.Threat.IntegrityViolation);
                fields.Add(GetDescription<Threat>(nameof(report.Threat.AccessibilityViolation)), report.Threat.AccessibilityViolation);
                fields.Add(GetDescription<Threat>(nameof(report.Threat.DateInclusion)), report.Threat.DateInclusion);
                fields.Add(GetDescription<Threat>(nameof(report.Threat.DateChange)), report.Threat.DateChange);

                AddTable(doc, fields, "Угроз");
            }
            if (report.Vulnerabilitie is not null)
            {
                Dictionary<string, string> fields = [];
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.Identifier)), report.Vulnerabilitie.Identifier);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.Name)), report.Vulnerabilitie.Name);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.Description)), report.Vulnerabilitie.Description);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.Vendor)), report.Vulnerabilitie.Vendor);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.NameSoftware)), report.Vulnerabilitie.NameSoftware);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.Version)), report.Vulnerabilitie.Version);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.Type)), report.Vulnerabilitie.Type);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.NameOperatingSystem)), report.Vulnerabilitie.NameOperatingSystem);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.Class)), report.Vulnerabilitie.Class);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.Date)), report.Vulnerabilitie.Date);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.CvssTwo)), report.Vulnerabilitie.CvssTwo);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.CvssThree)), report.Vulnerabilitie.CvssThree);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.DangerLevel)), report.Vulnerabilitie.DangerLevel);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.Measure)), report.Vulnerabilitie.Measure);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.Exploit)), report.Vulnerabilitie.Exploit);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.Information)), report.Vulnerabilitie.Information);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.Links)), report.Vulnerabilitie.Links);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.OtherIdentifier)), report.Vulnerabilitie.OtherIdentifier);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.OtherInformation)), report.Vulnerabilitie.OtherInformation);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.Incident)), report.Vulnerabilitie.Incident);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.OperatingMethod)), report.Vulnerabilitie.OperatingMethod);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.EliminationMethod)), report.Vulnerabilitie.EliminationMethod);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.DescriptionCwe)), report.Vulnerabilitie.DescriptionCwe);
                fields.Add(GetDescription<VulnerabilitieFstec>(nameof(report.Vulnerabilitie.Cwe)), report.Vulnerabilitie.Cwe);

                fields.Add(GetDescription<VulnerabilitieFstec>(report.Vulnerabilitie.Identifier), report.Vulnerabilitie.Identifier);
                AddTable(doc, fields, "Уязвимостей");
            }
            if (report.Exploit is not null)
            {
                Dictionary<string, string> fields = [];

                fields.Add(GetDescription<Exploit>(nameof(report.Exploit.Id)), report.Exploit.Id);
                fields.Add(GetDescription<Exploit>(nameof(report.Exploit.Name)), report.Exploit.Name);
                fields.Add(GetDescription<Exploit>(nameof(report.Exploit.Cve)), report.Exploit.Cve);
                fields.Add(GetDescription<Exploit>(nameof(report.Exploit.Description)), report.Exploit.Description);
                fields.Add(GetDescription<Exploit>(nameof(report.Exploit.Author)), report.Exploit.Author);
                fields.Add(GetDescription<Exploit>(nameof(report.Exploit.Type)), report.Exploit.Type);
                fields.Add(GetDescription<Exploit>(nameof(report.Exploit.Platform)), report.Exploit.Platform);
                fields.Add(GetDescription<Exploit>(nameof(report.Exploit.DatePublication)), report.Exploit.DatePublication);

                AddTable(doc, fields, "Эксплойтов");
            }
            if (report.Tactic is not null)
            {
                Dictionary<string, string> fields = [];

                fields.Add(report.Tactic.Name, report.Tactic.Description);
                foreach (var technique in report.Tactic.Techniques)
                {
                    fields.Add(technique.Name, technique.Description);
                }

                AddTable(doc, fields, "Техник и тактик");
            }
            if (report.DynamicRecords is not null)
            {
                foreach (var record in report.DynamicRecords)
                {
                    AddTable(doc, record.Fields, record.NameDatadase);
                }
            }
        }

        private static void AddTable(DocX doc, Dictionary<string, string> fields, string nameDatadase)
        {
            var columnCount = 2;
            var rowCount = fields.Count + 1;

            var table = doc.AddTable(rowCount, columnCount);
            table.AutoFit = AutoFit.Contents;

            table.Rows[0].MergeCells(0, 1);
            table.Rows[0].Cells[0].Paragraphs[0].Append($"Источник знания БД: {nameDatadase}").Bold().Alignment = Alignment.center;
            var rowIterator = 1;
            foreach (var field in fields)
            {
                table.Rows[rowIterator].Cells[0].Paragraphs[0].Append($"{field.Key}").Alignment = Alignment.left;
                table.Rows[rowIterator].Cells[1].Paragraphs[0].Append($"{field.Value}").Alignment = Alignment.center;
                rowIterator++;
            }
            doc.InsertTable(table);
            doc.InsertParagraph();
            doc.InsertParagraph();
        }

        private static string GetDescription<T>(string memberName)
        {
            var type = typeof(T);

            var prop = type.GetProperty(memberName);
            if (prop != null)
            {
                var attr = prop.GetCustomAttribute<DescriptionAttribute>();
                return attr?.Description ?? memberName;
            }

            var field = type.GetField(memberName);
            if (field != null)
            {
                var attr = field.GetCustomAttribute<DescriptionAttribute>();
                return attr?.Description ?? memberName;
            }

            return memberName;
        }
    }
}