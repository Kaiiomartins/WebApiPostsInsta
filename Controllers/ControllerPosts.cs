using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using PostsWebApi.Models;
using PostsWebApi.Servicos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace PostsWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ControllerPosts : ControllerBase
{
    private readonly ServicesPosts _servicesPosts;


    private readonly IConfiguration _configuration;

    public ControllerPosts(ServicesPosts servicesPosts, IConfiguration configuration)
    {
        _servicesPosts = servicesPosts;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Users usuarioLogin)
    {

        var usuario = await _servicesPosts.GetUserByUserName(usuarioLogin.UserName);

        if (usuario == null || usuario.Password != usuarioLogin.Password)
        {
            return Unauthorized(new { mensagem = "Usuário ou senha inválidos." });
        }

        var token = GerarToken(usuario);

        return Ok(new
        {
            token = token,
            usuario = new
            {
                usuario.Id,
                usuario.UserName,
                usuario.Email
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPostById(int id)
    {
        var post = await _servicesPosts.GetPostById(id);

        if (post == null)
            return NotFound(new { mensagem = "Post não encontrado." });

        return Ok(post);
    }

    [HttpPost("texto")]
    public async Task<Posts> CreatePostText([FromBody] Posts posts)
    {
        var post = await _servicesPosts.GetPostById(posts.Id);

        if (post != null) {

            throw new Exception("Jà existe esse post");
        }

        return await _servicesPosts.CreatePosts(posts);

    }

    [HttpPost("Imagem")]
    public async Task<IActionResult> CreatePostImagem(
      [FromForm] int userId,
      [FromForm] string description,
      [FromForm] string postType,
      [FromForm] DateTime postDate,
      [FromForm] IFormFile imagem)
    {
        var post = new Posts
        {
            UserId = userId,
            Description = description,
            PostType = postType,
            PostDate = postDate
        };


        var criado = await _servicesPosts.CreatePostComImagemAsync(post, imagem);

        return Ok(criado);
    }

    [HttpGet("Imagem/Visualizar/{postId}")]
    public async Task<IActionResult> VisualizarImagem(int postId)
    {
        var caminhoRelativo = await _servicesPosts.GetCaminhoImagemAsync(postId);

        if (string.IsNullOrEmpty(caminhoRelativo))
            return NotFound("Imagem não encontrada.");

        var caminhoFisico = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", caminhoRelativo.TrimStart('/'));

        if (!System.IO.File.Exists(caminhoFisico))
            return NotFound("Arquivo não existe no disco.");

        var extensao = Path.GetExtension(caminhoFisico).ToLowerInvariant();
        var contentType = extensao switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };

        var bytes = await System.IO.File.ReadAllBytesAsync(caminhoFisico);
        return File(bytes, contentType);
    }



    [HttpPut]
    public async Task<IActionResult> PutPost([FromBody] Posts posts)
    {
        var postExistente = await _servicesPosts.GetPostById(posts.Id);

        if (postExistente == null)
        {
            return NotFound(new { mensagem = "Post não encontrado." });
        }

        var postAtualizado = await _servicesPosts.UpdatePostAsync(posts);
        return Ok(postAtualizado);
    }
    [HttpDelete("{Id}")]
    public async Task<IActionResult> DeletePosts( int id)
    {
        var postExistente = await _servicesPosts.GetPostById(id);

        if (postExistente == null)
        {
            return NotFound(new { mensagem = "Post não encontrado." });
        }

        var postDeletado = await _servicesPosts.DeletesPostAsync(id);

        return Ok(postDeletado);
    }

    public string GerarToken(Users user)
    {
        var claims = new[]
        {
            new Claim("UserId", user.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


}
