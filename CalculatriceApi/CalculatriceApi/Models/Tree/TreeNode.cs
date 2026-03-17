using System.Text.Json.Serialization;

namespace CalculatriceApi.Models.Tree;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(TreeNumNode), "number")]
[JsonDerivedType(typeof(TreeBinNode), "binairy")]
[JsonDerivedType(typeof(TreeFuncNode), "function")]
public abstract record TreeNode(double Value);