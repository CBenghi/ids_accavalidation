using IDSAuditToolLib.Utility;
using IdsLib;
using IDSValidator.Test.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Reflection;

namespace IDSValidator.Test
{
    [TestClass]
    public class UnitTest_IDSDocs
    {        
        private static string GetAssemblyPath() => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        private static string GetXMLString(string fileWithAbsPath)
        {            
            string[] files = File.ReadAllLines(fileWithAbsPath);
            return string.Join(string.Empty, files).Trim();
        } 

        [TestMethod]
        public void Document_With_Exception()
        {
            var assemblyPath = GetAssemblyPath();
            string path = Path.Combine(assemblyPath, @$"idsFileCollection");
            var files = Directory.EnumerateFiles(path);

            var exceptionCounter = 0;
            List<Exception> exceptions = [];
            foreach (var file in files.OrderBy(x => x))
            {
                Debug.WriteLine($"=== Processing file: {file}");
				var xmlString = GetXMLString(file);                
                    
                var stream = IDSValidatorUtility.GenerateStreamFromString(xmlString);
                
                string streamContent;
				using (var reader = new StreamReader(stream))
				{
					streamContent = reader.ReadToEnd();
                    stream.Position = 0; // Reset the stream position for further processing
                    var lns = streamContent.Split([Environment.NewLine], StringSplitOptions.None);
					var lineCount = lns.Length;
                    Debug.WriteLine($"Line count is {lineCount}");
                    Assert.IsFalse(lineCount == 1, "We are expecting multiple lines in the stream.");
				}

				var singleAuditOptions = new SingleAuditOptions
                {
                    IdsVersion = IdsLib.IdsSchema.IdsNodes.IdsVersion.Ids1_0,
                    OmitIdsContentAudit = false,
                };
                var idsEditorLogger = new IDSEditorLogger();

                var auditStatus = Audit.Run(stream, singleAuditOptions, idsEditorLogger);

                Debug.WriteLine($"File: {file} - IsValid: {auditStatus == Audit.Status.Ok}");
                Debug.WriteLine($"MessageStack: {string.Join(", ", idsEditorLogger.MessaggesStack)}");
                exceptionCounter += idsEditorLogger.MessaggesStack.Count;                
            }

            exceptions.ForEach(x => Debug.WriteLine(x)); 
            Assert.AreEqual(1, exceptionCounter);
        }
    }
}