using affolterNET.Data.DtoHelper.Database;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace affolterNET.Data.DtoHelper.CodeGen
{
    public class PropertyGenerator
    {
        private readonly Column col;

        public PropertyGenerator(Column col)
        {
            this.col = col;
        }

        private AccessorDeclarationSyntax GetAccessor()
        {
            return SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        private AccessorDeclarationSyntax SetAccessor()
        {
            return SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        public PropertyDeclarationSyntax Generate()
        {
            var type = SyntaxFactory.ParseTypeName(col.PropertyType);
            var name = col.PropertyName;
            var propertyDeclaration = SyntaxFactory.PropertyDeclaration(type, name)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(GetAccessor(), SetAccessor());

            // primary key
            if (col.IsPK)
            {
                propertyDeclaration = propertyDeclaration.WithAttributeLists(
                    SyntaxFactory.SingletonList(
                        SyntaxFactory.AttributeList(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("Key"))))));
            }

            return propertyDeclaration;
        }
    }
}