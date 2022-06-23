using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebStore.Domain.DTO.Identity;

public abstract class ClaimDTO: UserDTO
{
    public IEnumerable<Claim> Claims { get; init; } = null!;
}

public class ReplaceClaimDTO : UserDTO
{
    public Claim Claim { get; init; } = null!;
    public Claim NewClaim { get; init; } = null!;
}