using System;
using System.Globalization;
using System.IdentityModel.Claims;
using System.IdentityModel.Tokens;

namespace Sdl.Enterprise2.MultiTerm.Client.Security
{
	public class TokenIdentity : UniqueIdentity
	{
		private readonly Claim[] claims;

		private readonly SecurityToken token;

		public string TokenId => token.Id;

		public Claim[] Claims => claims;

		public sealed override bool IsAuthenticated
		{
			get
			{
				if (!string.IsNullOrEmpty(base.Name))
				{
					return base.Id != Guid.Empty;
				}
				return false;
			}
		}

		public TokenIdentity(Claim[] claims, SecurityToken token)
		{
			if (claims == null || claims.Length == 0)
			{
				throw new ArgumentNullException("claims");
			}
			if (token == null)
			{
				throw new ArgumentNullException("token");
			}
			foreach (Claim claim in claims)
			{
				if (claim.ClaimType.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", StringComparison.OrdinalIgnoreCase))
				{
					base.Name = Convert.ToString(claim.Resource, CultureInfo.InvariantCulture);
				}
				else if (claim.ClaimType.Equals("http://sdl.com/identity/2010/claims/uniqueid", StringComparison.OrdinalIgnoreCase))
				{
					base.Id = new Guid(Convert.ToString(claim.Resource, CultureInfo.InvariantCulture));
				}
				else if (claim.ClaimType.Equals("http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod", StringComparison.OrdinalIgnoreCase))
				{
					base.AuthenticationType = Convert.ToString(claim.Resource, CultureInfo.InvariantCulture);
				}
			}
			this.claims = claims;
			this.token = token;
		}
	}
}
