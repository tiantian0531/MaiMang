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
        // 规则1: 异步方法不能返回 void
        private const string AsyncVoidRuleId = "CODE001";
        private static readonly DiagnosticDescriptor AsyncVoidRule = new DiagnosticDescriptor(
            id: AsyncVoidRuleId,
            title: "异步方法禁止返回 void",
            messageFormat: "异步方法 '{0}' 必须返回 Task 或 Task<T> ,以避免未观察到的异常",
            category: "Reliability",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );

        // 规则2: 禁止方法内嵌套本地方法
        private const string NestedLocalFunctionRuleId = "CODE002";
        private static readonly DiagnosticDescriptor NestedLocalFunctionRule = new DiagnosticDescriptor(
            id: NestedLocalFunctionRuleId,
            title: "禁止在方法内定义本地方法",
            messageFormat: "方法 '{0}' 包含嵌套的本地方法 ,应提取为私有方法以提高可读性",
            category: "Maintainability",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(AsyncVoidRule, NestedLocalFunctionRule);

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
            //if (context.Node is MethodDeclarationSyntax method)
            //{
            //    if (method.Modifiers.Any(x => x.IsKind(SyntaxKind.AsyncKeyword)) && method.ReturnType.ToString() == "void")
            //    {
            //        var diagnostic = Diagnostic.Create(Rule, context.Node.GetLocation(), "异步方法不能返回void");

            //        context.ReportDiagnostic(diagnostic);
            //    }
            //}

            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            var semanticModel = context.SemanticModel;

            // 检查是否标记为 async
            if (!methodDeclaration.Modifiers.Any(SyntaxKind.AsyncKeyword)) return;

            // 获取方法符号
            var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration);
            if (methodSymbol == null) return;


            // 检查返回类型是否为 void
            if (methodSymbol.ReturnsVoid)
            {
                var diagnostic = Diagnostic.Create(
                    AsyncVoidRule,
                    methodDeclaration.Identifier.GetLocation(),
                    methodSymbol.Name
                );
                context.ReportDiagnostic(diagnostic);
            }
        }

        /// <summary>
        /// 方法不能嵌套本地方法
        /// </summary>
        /// <param name="context"></param>
        private static void MethodNotIncludeMethod(SyntaxNodeAnalysisContext context)
        {
            //if (context.Node is MethodDeclarationSyntax method)
            //{
            //    var semanticModel = context.SemanticModel;

            //    // 忽略接口方法和抽象方法
            //    var methodSymbol = semanticModel.GetDeclaredSymbol(method);

            //    // 查找方法体内的所有局部函数定义
            //    var localFunctions = method.DescendantNodes().OfType<LocalFunctionStatementSyntax>();
            //    foreach (var localFunction in localFunctions)
            //    {
            //        var diagnostic = Diagnostic.Create(
            //         NestedLocalFunctionRule,
            //         localFunction.Identifier.GetLocation(),
            //         methodSymbol?.Name ?? "未知方法"
            //     );
            //        context.ReportDiagnostic(diagnostic);
            //    }
            //}

            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            var semanticModel = context.SemanticModel;

            // 忽略接口方法和抽象方法
            var methodSymbol = semanticModel.GetDeclaredSymbol(methodDeclaration);
            if (methodSymbol?.ContainingType?.TypeKind == TypeKind.Interface ||
                methodSymbol?.IsAbstract == true)
                return;

            // 查找所有本地方法定义
            var localFunctions = methodDeclaration.DescendantNodes()
                .OfType<LocalFunctionStatementSyntax>();

            foreach (var localFunction in localFunctions)
            {
                var diagnostic = Diagnostic.Create(
                    NestedLocalFunctionRule,
                    localFunction.Identifier.GetLocation(),
                    methodSymbol?.Name ?? "未知方法"
                );
                context.ReportDiagnostic(diagnostic);
            }

        }
    }
}