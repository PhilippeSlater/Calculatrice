using CalculatriceApi.Models.Tree;

namespace CalculatriceApi.Models.Responses;

public record ExprResponse(double Result, TreeNode Tree, List<TokenDTO> Tokens);

public record TokenDTO(string Value, string Type);