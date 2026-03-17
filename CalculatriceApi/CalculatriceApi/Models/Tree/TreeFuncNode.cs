namespace CalculatriceApi.Models.Tree;

public record TreeFuncNode(string Function, TreeNode Argument, double Eval) : TreeNode(Eval);