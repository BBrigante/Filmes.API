using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class FilmeController : ControllerBase
{

    private FilmeContext _context;
    private IMapper _mapper;  

    public FilmeController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Adiciona ao banco de dados
    /// </summary>
    /// <param name="filmeDto">Objeto com os campos necessários para criação de um filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AdicionaFilme(
        [FromBody] CreateFilmeDto filmeDto)
    {
        Filme filme = _mapper.Map<Filme>(filmeDto);
        _context.Filmes.Add(filme);
        _context.SaveChanges();
        return CreatedAtAction(nameof(RecuperaFilmePorId),
            new { id = filme.Id }, 
            filme);
    }

    /// <summary>
    /// Definições de busca com paginação ao banco de dados
    /// </summary>
    /// <param name="skip">Objeto com os campos necessários de busca mínima da paginação de um filme</param>
    /// <param name="take">Objeto com os campos necessários de busca máxima da paginação de um filme</param>
    /// <returns>IEnumerable</returns>
    /// <response code="200">Caso inserção seja feita com sucesso</response>
    [HttpGet]
    public IEnumerable<ReadFilmeDto> RecuperaFilme([FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip
            (skip).Take(take));
    }

    /// <summary>
    /// Definições de busca por id ao banco de dados 
    /// </summary>
    /// <param name="id">Objeto com os campos necessários de busca por id de um filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso inserção seja feita com sucesso</response>
    [HttpGet("{id}")]
    public IActionResult RecuperaFilmePorId(int id)
    {
        var filme = _context.Filmes.
            FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();
        var filmeDto = _mapper.Map<ReadFilmeDto>(filme);
        return Ok(filmeDto);
    }

    /// <summary>
    /// Definições de alteração de todos campos ao banco de dados 
    /// </summary>
    /// <param name="filmeDto">Objeto com os todos campos necessários de Alteração de um filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Caso inserção seja feita com sucesso</response>
    [HttpPut("{id}")]
    public IActionResult AtualizaFilme(int id, 
        [FromBody] UpdateFilmeDto filmeDto) 
    {
        var filme = _context.Filmes.FirstOrDefault(
            filme => filme.Id == id);
        if (filme == null) return NotFound();
        _mapper.Map(filmeDto, filme);
        _context.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Definições de alteração de um campo ao banco de dados 
    /// </summary>
    /// <param name="patch">Objeto com um campo necessário de Alteração de um filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Caso inserção seja feita com sucesso</response>
    [HttpPatch("{id}")]
    public IActionResult AtualizaFilmeParcial(int id,
        JsonPatchDocument<UpdateFilmeDto> patch)
    {
        var filme = _context.Filmes.FirstOrDefault(
            filme => filme.Id == id);
        if (filme == null) return NotFound();

        var filmeParaAtualizar = _mapper.Map<UpdateFilmeDto>(filme);

        patch.ApplyTo(filmeParaAtualizar, ModelState);

        if (!TryValidateModel(filmeParaAtualizar))
        {
            return ValidationProblem(ModelState);
        }

        _mapper.Map(filmeParaAtualizar, filme);
        _context.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Deleta ao banco de dados 
    /// </summary>
    /// <param name="id">Objeto para deletar um filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Caso inserção seja feita com sucesso</response>
    [HttpDelete("{id}")]
    public IActionResult DeletaFilme(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(
            filme => filme.Id == id);
        if (filme == null) return NotFound();
        _context.Remove(filme);
        _context.SaveChanges();
        return NoContent();
    }
}
