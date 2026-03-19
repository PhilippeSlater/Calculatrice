using CalculatriceApi.Models.Tree;

namespace CalculatriceApi.Services;

public class Parser(List<Token> tokens)
{
    private int _pos = 0;

    private Token? Peek()    => _pos < tokens.Count ? tokens[_pos] : null;
    private Token  Consume() => tokens[_pos++];

    internal Node ParseInternal() => ParseExpr();

    // Niveau 1 : + et - (priorité basse)
    private Node ParseExpr()
    {
        var node = ParseTerm();
        while (Peek()?.Type is TokenType.Plus or TokenType.Minus) //Termine lorsque le prochain token n'est pas + ou - ou est null (fin de l'expression)
        {
            string op = Consume().Type == TokenType.Plus ? "+" : "-";
            node = new BinNode(op, node, ParseTerm ());
        }
        return node;
    }

    // Niveau 2 : * et /
    private Node ParseTerm()
    {
        var node = ParsePower();
        while (Peek()?.Type is TokenType.Multiply or TokenType.Divide)
        {
            string op = Consume().Type == TokenType.Multiply ? "*" : "/";
            node = new BinNode(op, node, ParsePower());
        }
        return node;
    }

    // Niveau 3 : ^ (associatif à droite)
    private Node ParsePower()
    {
        var node = ParseFactor();
        if (Peek()?.Type == TokenType.Power)
        {
            Consume();
            node = new BinNode("^", node, ParsePower());
        }
        return node;
    }

    // Niveau 4 : nombre, parenthèses, fonctions
    private Node ParseFactor()
    {
        var tok = Peek() ?? throw new Exception("Expression incomplète");
        //Gerer si valeur negatif, on ajouter * -1 devant le nombre ou l'expression
        // -1, -(2+3), -sqrt(4)
        if (tok.Type == TokenType.Minus)
        {
            Consume();                        // consomme le '-'
            var operand = ParseFactor();
            return new BinNode("*", new NumNode(-1), operand);
        }
        // +3 = 3
        if (tok.Type == TokenType.Plus)
        {
            Consume();                        // consomme le '+'
            return ParseFactor();
        }
        if (tok.Type == TokenType.Number)
        {
            Consume();
            return new NumNode(tok.Value);
        }

        if (tok.Type == TokenType.LeftParenthesis)
        {
            Consume();
            var node = ParseExpr();
            if (Peek()?.Type != TokenType.RightParenthesis)
                throw new Exception("Parenthèse ')' manquante");
            Consume();
            return node;
        }

        if (tok.Type == TokenType.Sqrt)
        {
            Consume();
            if (Peek()?.Type != TokenType.LeftParenthesis)
                throw new Exception("'(' attendu après sqrt");
            Consume();
            var arg = ParseExpr();
            if (Peek()?.Type != TokenType.RightParenthesis)
                throw new Exception("Parenthèse ')' manquante après sqrt");
            Consume();
            return new FuncNode("sqrt", arg);
        }

        if (tok.Type == TokenType.Log)
        {
            Consume();
            if (Peek()?.Type != TokenType.LeftParenthesis)
                throw new Exception("'(' attendu après log");
            Consume();
            var arg = ParseExpr();
            if (Peek()?.Type != TokenType.RightParenthesis)
                throw new Exception("Parenthèse ')' manquante après log");
            Consume();
            return new FuncNode("log", arg);
        }

        if (tok.Type == TokenType.Sin)
        {
            Consume();
            if (Peek()?.Type != TokenType.LeftParenthesis)
                throw new Exception("'(' attendu après sin");
            Consume();
            var arg = ParseExpr();
            if (Peek()?.Type != TokenType.RightParenthesis)
                throw new Exception("Parenthèse ')' manquante après sin");
            Consume();
            return new FuncNode("sin", arg);
        }

        if (tok.Type == TokenType.Cos)
        {
            Consume();
            if (Peek()?.Type != TokenType.LeftParenthesis)
                throw new Exception("'(' attendu après cos");
            Consume();
            var arg = ParseExpr();
            if (Peek()?.Type != TokenType.RightParenthesis)
                throw new Exception("Parenthèse ')' manquante après cos");
            Consume();
            return new FuncNode("cos", arg);
        }

        if (tok.Type == TokenType.Tan)
        {
            Consume();
            if (Peek()?.Type != TokenType.LeftParenthesis)
                throw new Exception("'(' attendu après tan");
            Consume();
            var arg = ParseExpr();
            if (Peek()?.Type != TokenType.RightParenthesis)
                throw new Exception("Parenthèse ')' manquante après tan");
            Consume();
            return new FuncNode("tan", arg);
        }

        throw new Exception($"Token inattendu : {tok.StrValue}");
    }
}