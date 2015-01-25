using System.Collections.Generic;
using Rubberduck.VBA.Grammar;

namespace Rubberduck.VBA.Nodes
{
    public class VariableDeclarationNode : Node
    {
        public VariableDeclarationNode(VisualBasic6Parser.VariableStmtContext context, string scope)
            :base(context, scope, null, new List<Node>())
        {
            foreach (var variable in context.variableListStmt().variableSubStmt())
            {
                AddChild(new VariableNode(variable, scope, context.visibility(), context.DIM() != null || context.STATIC() != null));
            }
        }
    }

    public class VariableNode : Node
    {
        private static readonly IDictionary<string, string> TypeSpecifiers = new Dictionary<string, string>
        {
            { "%", ReservedKeywords.Integer },
            { "&", ReservedKeywords.Long },
            { "@", ReservedKeywords.Decimal },
            { "!", ReservedKeywords.Single },
            { "#", ReservedKeywords.Double },
            { "$", ReservedKeywords.String }
        };

        public VariableNode(VisualBasic6Parser.VariableSubStmtContext context, string scope, 
                            VisualBasic6Parser.VisibilityContext visibility, bool isLocal = true)
            : base(context, scope)
        {
            _name = context.ambiguousIdentifier().GetText();
            if (context.asTypeClause() == null)
            {
                if (context.typeHint() == null)
                {
                    _isImplicitlyTyped = true;
                    _typeName = ReservedKeywords.Variant;
                }
                else
                {
                    var hint = context.typeHint().GetText();
                    _isUsingTypeHint = true;
                    _typeName = TypeSpecifiers[hint];
                }
            }
            else
            {
                _typeName = context.asTypeClause().type().GetText();
            }

            _accessibility = isLocal ? VBAccessibility.Private : visibility.GetAccessibility();
        }

        private readonly string _name;
        public string Name { get { return _name; } }

        private readonly string _typeName;
        public string TypeName { get { return _typeName; } }

        private readonly bool _isImplicitlyTyped;
        public bool IsImplicitlyTyped { get { return _isImplicitlyTyped; } }

        private bool _isUsingTypeHint;
        public bool IsUsingTypeHint { get { return _isUsingTypeHint; } }

        private readonly VBAccessibility _accessibility;
        public VBAccessibility Accessibility { get { return _accessibility; } }
    }
}