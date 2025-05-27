using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace MaiMangAnalyzer
{
    [DiagnosticAnalyzer(Microsoft.CodeAnalysis.LanguageNames.CSharp)]
    public class MaiMangDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            id: "Code001",
            title: "示例规则标题",
            messageFormat: "代码规范检查：{0}",
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AsyncNotRetVoidAnalyze, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(MethodNotIncludeMethod, SyntaxKind.MethodDeclaration);
        }

        /// <summary>
        /// 异步方法不能返回void
        /// </summary>
        /// <param name="context"></param>
        private static void AsyncNotRetVoidAnalyze(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is MethodDeclarationSyntax method)
            {
                if (method.Modifiers.Any(x => x.IsKind(SyntaxKind.AsyncKeyword)) && method.ReturnType.ToString() == "void")
                {
                    var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation(), "异步方法不能返回void");

                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        /// <summary>
        /// 方法不能嵌套本地方法
        /// </summary>
        /// <param name="context"></param>
        private static void MethodNotIncludeMethod(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is MethodDeclarationSyntax method)
            {
                // 查找方法体内的所有局部函数定义
                var localFunctions = method.DescendantNodes().OfType<LocalFunctionStatementSyntax>();
                foreach (var localFunction in localFunctions)
                {
                    var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation(), "方法中不能嵌套本地方法");
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}