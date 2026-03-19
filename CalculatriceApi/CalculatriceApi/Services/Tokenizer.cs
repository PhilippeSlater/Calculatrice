using CalculatriceApi.Models.Responses;

namespace CalculatriceApi.Services;


// token interne utilise seulement durant le parsing
public enum TokenType
{
    Number,
    Plus,
    Minus,
    Multiply,
    Divide,
    Power,
    Sqrt,
    Sin,
    Cos,
    Tan,
    Log,
    LeftParenthesis,
    RightParenthesis
}

public record Token(TokenType Type, double Value = 0, string StrValue = "");

public class Tokenizer
{
    public static List<Token> Tokenize(string input)
    {
        input = input.Replace(",", "."); // Remplacer les virgules par des points pour faciliter le parsing
        var tokens = new List<Token>();
        int i = 0;
        while (i < input.Length)
        {
            char c = input[i];

            //Logique pour chaque operateur et fonction supportee

            //Si caratere vide, alors on passe au suivant
            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }
            
            //Si ses un nombre a virgule flottante
            if (char.IsDigit(c) || c == '.')
            {
                int start = i;
                //Si on entre ..2 on recoit l'erreur seulement au convertisage en double
                //Pourrais valider le dernier caractere pour eviter les erreurs de format de nombre, mais c'est plus simple de laisser le double.TryParse gerer ca et lancer une exception si le format est invalide
                while (i < input.Length && (char.IsDigit(input[i]) || input[i] == '.')) //TODO : Permettre les notations scientifiques (ex: 1.23e-4)
                    i++;
                var numberStr = input.Substring(start, i - start);
                if (double.TryParse(numberStr, out double number))
                {
                    tokens.Add(new Token(TokenType.Number, number, numberStr));
                    continue;
                }
                else
                {
                    throw new Exception($"Format de nombre invalide: {numberStr}");
                }
            }

            //Si ses une function, elle commence par une lettre
            //TODO: Permettre des functions qui commencerais par des chiffres, doit modifier le elle de IsDigit
            //Pourrais penser a ajouter une notion de constante (ex: pi, e) qui serait traité comme des fonctions sans argument
            if (char.IsLetter(c))
            {
                int start = i;
                while (i < input.Length && char.IsLetter(input[i]))
                    i++;
                var funcStr = input.Substring(start, i - start).ToLower();
                var tokenType = funcStr switch  
                {
                    "sin" => TokenType.Sin,
                    "cos" => TokenType.Cos,
                    "tan" => TokenType.Tan,
                    "log" => TokenType.Log,
                    "sqrt" => TokenType.Sqrt,
                    _ => throw new Exception($"Fonction inconnue: {funcStr}")
                };
                tokens.Add(new Token(tokenType, StrValue: funcStr));
                continue;
            }

            // Si ce n'est pas un nombre, ni une fonction, ni un espace, alors c'est un operateur ou une parenthese
            TokenType type = c switch
            {
                '+' => TokenType.Plus,
                '-' => TokenType.Minus,
                '*' => TokenType.Multiply,
                '/' => TokenType.Divide,
                '^' => TokenType.Power,
                '(' => TokenType.LeftParenthesis,
                ')' => TokenType.RightParenthesis,
                _   => throw new Exception($"Caractere inconnu : '{c}'")
            };
            tokens.Add(new Token(type, StrValue: c.ToString()));
            i++;
        }
        return tokens;
    }

    //Convertir les tokens internes en DTO pour les envoyer au frontend, en gardant seulement les informations necessaires (ex: type et valeur sous forme de string)    
    public static List<TokenDTO> ToDTOs(List<Token> tokens)
    {
        return tokens.Select(t => new TokenDTO(t.StrValue, t.Type.ToString())).ToList();
    }
}