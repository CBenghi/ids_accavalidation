using IdsLib.IdsSchema.IdsNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDSAuditToolLib.Validator
{
    public class IDSMessageAudit
    {
        public int? Code { get; set; } = null;

        public string Message { get; set; } = string.Empty;

        public string NodePath { get; set; } = string.Empty;

        public string NodeType { get; set; } = string.Empty;

        public int StartLineNumber { get; set; } = 0;

        public int StartLinePosition { get; set; } = 0;
    }
}
