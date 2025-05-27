using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Diagnostics;

namespace MaiMangAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NamingStyleAnalyzer : DiagnosticAnalyzer
    {
        // 规则1: 方法名必须大写
        private const string MethodNamingRuleId = "MMSTYLE001";
        private static readonly DiagnosticDescriptor MethodNamingRule = new DiagnosticDescriptor(
            id: MethodNamingRuleId,
            title: "方法名必须首字母大写",
            messageFormat: "方法名 '{0}' 首字母必须大写",
            category: "Naming",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );

        // 规则2: 参数名必须小写
        private const string ParameterNamingRuleId = "MMSTYLE002";
        private static readonly DiagnosticDescriptor ParameterNamingRule = new DiagnosticDescriptor(
            id: ParameterNamingRuleId,
            title: "形参名首字母必须小写",
            messageFormat: "参数名 '{0}' 首字母必须小写",
            category: "Naming",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(MethodNamingRule, ParameterNamingRule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // 注册方法名和参数名的检查
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeParameter, SyntaxKind.Parameter);
        }

        // 检查方法名是否大写
        private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            var methodName = methodDeclaration.Identifier.Text;
            if (methodName.Length == 0 || !char.IsUpper(methodName[0]))
            {
                var diagnostic = Diagnostic.Create(
                    MethodNamingRule,
                    methodDeclaration.Identifier.GetLocation(),
                    methodName
                );
                context.ReportDiagnostic(diagnostic);
            }
        }

        // 检查参数名是否小写
        private void AnalyzeParameter(SyntaxNodeAnalysisContext context)
        {
            var parameter = (ParameterSyntax)context.Node;
            var parameterName = parameter.Identifier.Text;

            if (parameterName.Length == 0 || !char.IsLower(parameterName[0]))
            {
                var diagnostic = Diagnostic.Create(
                    ParameterNamingRule,
                    parameter.Identifier.GetLocation(),
                    parameterName
                );
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
