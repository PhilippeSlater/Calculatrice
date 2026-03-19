using CalculatriceApi.Models.Tree;
using CalculatriceApi.Models.Responses;

namespace CalculatriceApi.Services;

public class ExprService
{
    public (double Result, TreeNode Tree, List<TokenDTO> Tokens) Evaluate(string input)
    {
        var internalTokens = Tokenizer.Tokenize(input);
        var tokenDtos      = Tokenizer.ToDTOs(internalTokens);
        var tree           = new Parser(internalTokens).ParseInternal();
        var result         = tree.Eval();
        var treeJson        = tree.ToTreeNode();

        return (result, treeJson, tokenDtos);
    }
}