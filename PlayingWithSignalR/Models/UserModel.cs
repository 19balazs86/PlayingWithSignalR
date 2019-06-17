using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace PlayingWithSignalR.Models
{
  public class UserModel
  {
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public IEnumerable<string> Roles { get; private set; }

    public UserModel(Guid id, string name, IEnumerable<string> roles)
    {
      Id    = id;
      Name  = name;
      Roles = roles;
    }

    public UserModel(Guid id, string name) : this(id, name, Enumerable.Empty<string>()) {  }

    public UserModel(IEnumerable<Claim> claims)
    {
      List<string> roles = new List<string>();

      foreach (Claim claim in claims)
      {
        switch (claim.Type)
        {
          case ClaimTypes.NameIdentifier:
            Id = Guid.Parse(claim.Value);
            break;
          case ClaimTypes.Name:
            Name = claim.Value;
            break;
          case ClaimTypes.Role:
            roles.Add(claim.Value);
            break;
        }
      }

      Roles = roles;
    }

    public IEnumerable<Claim> ToClaims()
    {
      List<Claim> claims = new List<Claim>
      {
        new Claim(ClaimTypes.NameIdentifier, Id.ToString()), new Claim(ClaimTypes.Name, Name)
      };

      claims.AddRange(Roles.Select(role => new Claim(ClaimTypes.Role, role)));

      return claims;
    }
  }
}
