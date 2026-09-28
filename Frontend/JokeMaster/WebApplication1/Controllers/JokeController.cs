using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using WebApplication1.interfaces;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class JokeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private IJokeLogic jokeLogic;

        public List<string> jokes = new List<string>()
        {
            "Фермер купил петуха.\r\nВ первый день петух перетоптал всех кур в курятнике.\r\nНа второй день перетоптал всех уток, на третий день - гусей. На четвертый от петуха бегали все птицы на ферме.\r\nНа пятый день фермер вышел во двор и увидел безжизненное тело петуха, над которым вились стервятники.\r\n- Эх, доигрался петушок, - сказал фермер.\r\nТут петух приоткрыл один глаз и прошептал, - Тсс, мужик, не шуми, пусть только они сядут!",
            "Муж купил новый телевизор.\r\nЖена:\r\n— Ой, а почему это на коробке бокал нарисован?\r\nМуж:\r\n— Это значит, что покупку надо обмыть.",
            "Старый советский анекдот.\r\nВопрос армянскому радио:\r\n-Что будет, если в пустыне Сахара объявить социализм?\r\nОтвет:\r\n-Первые три года ничего, а потом начнутся перебои с песком...",
            "Мужик просидел весь день на рыбалке, но так ничего и не поймал.\r\nТогда по пути домой он заходит на местный рынок, идет в рыбный ряд и хочет купить у продавца пару карпов.\r\nА тот ему говорит: — Мне тут звонила твоя жена и сказала, что сегодня она бы предпочла форель."
        };

        public JokeController(IJokeLogic jokeLogic)
        {
            this.jokeLogic = jokeLogic;
        }

        [Route("mainJoke")]
        [HttpGet]
        public string MainJoke()
        {
            return this.jokes[jokeLogic.getJokeindex()];
        }


        [HttpPost]
        [Route("main")]
        public void UpdateMainJoke([FromQuery] int index)
        {
            jokeLogic.setJokeIndex(index);
        }

        public async Task GetJoke([FromQuery] int index)
        {
            // Загружаем анекдот
            var joke = this.jokes[index];

            Response.ContentType = "text/html;charset=utf-8";

            // Заполняем таблицу заголовками http запроса
            System.Text.StringBuilder table = new System.Text.StringBuilder("<h2>Request Headers</h2>");
            foreach (var header in Request.Headers)
            {
                table.Append($"<div style=\"display: flex; flex-direction: row;\"><div>{header.Key}</div><div style=\"margin-left: 30px\">{header.Value}</div></div>");
            }
            table.Append("<div style=\"height:100px;\"></div>");

            //Заполняем рекламными данными
            var userAgent = Request.Headers.FirstOrDefault(e => e.Key == "User-Agent");

            if (userAgent.Value.Any(a => a.Contains("Mozilla")))
            {
                joke += " .\r\n Купите новый сяоми!";
            }

            if (userAgent.Value.Any(a => a.Contains("Chrome")))
            {
                joke += " .\r\n Купите новый iphone!";
            }

            table.Append(joke);


            await Response.WriteAsync(table.ToString());
        }

        [Route("reviewForm")]
        [HttpGet]
        public async Task reviewForm()
        {
            string content = @"<form method='post'>
                <label>Name:</label><br />
                <input name='name' /><br />
                <label>Age:</label><br />
                <input type='number' name='age' /><br />
                <input type='submit' value='Send' />
            </form>";
            Response.ContentType = "text/html;charset=utf-8";
            await Response.WriteAsync(content);
        }

        [Route("reviewForm")]
        [HttpPost]
        public IActionResult sendReview(Person person)
        {
            if (person.Name.Contains("Федор"))
            {
                return BadRequest();
            }
            return Ok();
        }

        public record class Person(string Name, int Age);

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
