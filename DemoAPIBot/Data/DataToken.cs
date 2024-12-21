

using DemoAPIBot.Models;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Mvc;

namespace DemoAPIBot.Data
{
    public static class DataToken
    {

        public async static Task StoreToken (DemoContext db, string userId, DateTime refreshExpiry, string refreshToken)
        {
            if (db.RefreshTokens.Where(rt => rt.UserID == userId).FirstOrDefault() != null)
                db.RefreshTokens.Remove(db.RefreshTokens.Where(rt => rt.UserID == userId).FirstOrDefault()); //anchec se può essere null, non è un problema

            await db.AddAsync(new RefreshToken
            {
                UserID = userId,
                ExpiryDate = refreshExpiry,
                Token = refreshToken
            });

            await db.SaveChangesAsync();
        }

        public static Task<bool> TokenIsValid(DemoContext db,string userId, string refreshToken)
        {
            return Task.FromResult(db.RefreshTokens.Where(t => t.UserID == userId && t.Token == refreshToken && t.ExpiryDate >= DateTime.UtcNow).Any());
        }
    }
}
