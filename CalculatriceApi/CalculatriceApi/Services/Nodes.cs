using CalculatriceApi.Models.Tree;

namespace CalculatriceApi.Services;

// Classe de base interne, jamais exposee au frontend, pour representer les noeuds de l'arbre d'expression
internal abstract class Node
{
    public abstract double Eval();
    public abstract TreeNode ToTreeNode();
}

// Represente un nombre dans l'arbre d'expression
class NumNode(double value) : Node
{
    public override double Eval() => value;
    public override TreeNode ToTreeNode() => new TreeNumNode(value, value);
}

// Represente une operation binaire dans l'arbre d'expression
// Ajouter dans le switch les operations supportees
class BinNode(string operatorm, Node left, Node right) : Node
{
    public override double Eval()
    {
        var leftVal = left.Eval();
        var rightVal = right.Eval();
        return operatorm switch
        {
            "+" => leftVal + rightVal,
            "-" => leftVal - rightVal,
            "*" => leftVal * rightVal,
            "/" => rightVal != 0 ? leftVal / rightVal : throw new DivideByZeroException(),
            "^" => Math.Pow(leftVal, rightVal),
            _ => throw new Exception($"Opérateur inconnu {operatorm}")
        };
    }

    public override TreeNode ToTreeNode() => new TreeBinNode(operatorm, left.ToTreeNode(), right.ToTreeNode(), Eval());
}

// Represente une fonction dans l'arbre d'expression
// Dans le switch, ajouter les fonctions supportees et leurs comportements (ex: domain de definition)
class FuncNode(string function, Node argument) : Node
{
    public override double Eval()
    {
        var argVal = argument.Eval();
        return function switch
        {
            "sin" => Math.Sin(argVal),
            "cos" => Math.Cos(argVal),
            "tan" => Math.Tan(argVal),
            "log" => argVal > 0 ? Math.Log(argVal) : throw new Exception("Logarithme de nombre négatif ou zéro n'est pas défini"),
            "sqrt" => argVal >= 0 ? Math.Sqrt(argVal) : throw new Exception("Racine carrée de nombre négatif"), // Ne supporte pas les nombres complexes
            _ => throw new Exception($"Fonction inconnue {function}")
        };
    }

    public override TreeNode ToTreeNode() => new TreeFuncNode(function, argument.ToTreeNode(), Eval());
}