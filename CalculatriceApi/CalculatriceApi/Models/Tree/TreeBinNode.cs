namespace CalculatriceApi.Models.Tree;

public record TreeBinNode(string Operator, TreeNode Left, TreeNode Right, double Eval) : TreeNode(Eval);