
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Microsoft.AspNetCore.Identity;

var hasher = new PasswordHasher<User>();
var password = "admin123";
var hash = hasher.HashPassword(new User(), password);
Console.WriteLine($"Hash para 'admin123':\n{hash}");