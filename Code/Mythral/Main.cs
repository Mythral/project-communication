using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunicationAPI
{
	public class AuthToken
	{
		public AuthToken(float _userID)
		{
			userID = _userID;
		}
		public float userID;
	}
}
