using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using writings_backend_dotnet.Controllers.Validation;
using writings_backend_dotnet.DB;
using writings_backend_dotnet.DTOs;
using writings_backend_dotnet.Models;
using writings_backend_dotnet.Services;

namespace writings_backend_dotnet.Controllers.VerseHandler
{
    [ApiController, Route("verse"), EnableRateLimiting(policyName: "StaticControllerRateLimiter")]
    public class VerseController(ApplicationDBContext db, ICacheService cacheService, ILogger<VerseController> logger) : ControllerBase
    {
        private readonly ApplicationDBContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ICacheService _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        private readonly ILogger<VerseController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpGet("{ScriptureNumber}/{SectionNumber}/{ChapterNumber}/{VerseNumber}")]
        public async Task<IActionResult> GetVerse([FromRoute] VerseValidatedModel dto)
        {
            VerseSimpleDTO data;

            string requestPath = Request.Path.ToString();

            VerseSimpleDTO? cache = await _cacheService.GetCachedDataAsync<VerseSimpleDTO>(requestPath);

            if (cache != null)  //Checking cache
            {
                _logger.LogInformation($"Cache data with URL {requestPath} is found. Sending.");
                return Ok(new { data = cache });
            }


            //Dynamic including, via .Select() can also be used.
            Verse? verse = await _db.Verse.AsNoTracking().IgnoreAutoIncludes() //AsNoTracking is used for avoid auto inclusion. 
                                            .Include(v => v.Chapter).ThenInclude(c => c.Section).ThenInclude(c => c.Scripture).ThenInclude(c => c.Meanings).ThenInclude(c => c.Language)
                                            .AsSingleQuery() //With the purpose of prevent Cartesian explosion. Reference : https://learn.microsoft.com/en-us/ef/core/querying/single-split-queries
                                            .Include(v => v.Chapter).ThenInclude(c => c.Section.Meanings).ThenInclude(c => c.Language)
                                            .AsSingleQuery()
                                            .Include(v => v.Chapter).ThenInclude(c => c.Meanings).ThenInclude(c => c.Language)
                                            .AsSingleQuery()
                                            .Include(v => v.Words).ThenInclude(w => w.Root).ThenInclude(r => r.Scripture)
                                            .AsSingleQuery()
                                            .Include(v => v.Transliterations).ThenInclude(t => t.Language)
                                            .AsSingleQuery()
                                            .Include(v => v.TranslationTexts).ThenInclude(t => t.Translation).ThenInclude(t => t.Language)
                                            .Include(v => v.TranslationTexts).ThenInclude(t => t.Translation).ThenInclude(t => t.TranslatorTranslations).ThenInclude(tt => tt.Translator).ThenInclude(t => t.Language)
                                            .AsSingleQuery()
                                            .Include(v => v.TranslationTexts).ThenInclude(t => t.FootNotes).ThenInclude(f => f.FootNoteText)
                                            .FirstOrDefaultAsync(v => v.Number == dto.VerseNumber);


            if (verse == null) return NotFound("There is no verse matches with this information.");

            data = verse.ToVerseSimpleDTO();

            await _cacheService.SetCacheDataAsync(requestPath, data);
            _logger.LogInformation($"Cache data for URL {requestPath} is renewing");

            return Ok(new { data });
        }

    }
}