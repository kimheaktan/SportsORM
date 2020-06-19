using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SportsORM.Models;
using Microsoft.EntityFrameworkCore;

namespace SportsORM.Controllers
{
    public class HomeController : Controller
    {

        private static Context context;

        public HomeController(Context DBContext)
        {
            context = DBContext;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            ViewBag.BaseballLeagues = context.Leagues
                .Where(l => l.Sport.Contains("Baseball"));
            return View();

        }

        [HttpGet("level_1")]
        public IActionResult Level1()
        { //convert them ToList() if we want the result to be list and use .Count in the View (ViewBag.someting.Count)
            // ~~~~~~~~~~~ Leagues ~~~~~~~~~~~
            // #1: Womens
            ViewBag.Women = context.Leagues.Where(w => w.Name.Contains("Women"));

            // List<League> WL = context.Leagues.Where(w => w.Name.Contains("Women")).ToList();

            // #2: Hockey
            ViewBag.Hockey = context.Leagues.Where(w => w.Sport.Contains("Hockey"));

            // List<League> Hockey = context.Leagues.Where(w => w.Sport.Contains("Hockey")).ToList();
            
            // #3: All sport but football
            ViewBag.NotFootball = context.Leagues.Where(nf => !nf.Sport.Contains("Football"));
            //OR// ViewBag.NotFootball = context.Leagues.Where(nf => nf.Sport != "Football");//but using Contains() safer in general in case the name is not just 1 word

            // #4: Conferences
            ViewBag.Con = context.Leagues.Where(con => con.Name.Contains("Conference"));

             // #5: Atlantic
            ViewBag.Atlantic = context.Leagues.Where(a => a.Name.Contains("Atlantic"));

            // ~~~~~~~~~~~ Teams ~~~~~~~~~~~
            // #6: Location Dallas
            ViewBag.Dallas = context.Teams.Where(d => d.Location.Contains("Dallas"));
            // #7: Name Raptors
            ViewBag.Raptors = context.Teams.Where(r => r.TeamName.Contains("Raptors"));
            // #8: Location "City"
            ViewBag.City = context.Teams.Where(c => c.Location.Contains("City"));
            // #9: Name starts with "T"
            ViewBag.T = context.Teams.Where(t => t.TeamName.Contains("T"));
            // #10: All Teams (Alphabetical Location)
            ViewBag.Ascending = context.Teams.OrderBy(l => l.Location);
            // #11: All Teams (Alphabetical Team Name, Reverse)
            ViewBag.Descending = context.Teams.OrderByDescending(l => l.TeamName);
            // ~~~~~~~~~~~ Players ~~~~~~~~~~~
            // #12: All Last Name "Cooper" Players
            ViewBag.Cooper = context.Players.Where(c => c.LastName.Contains("Cooper"));
            // #13: All First Name "Joshua" Players
            ViewBag.Joshua = context.Players.Where(j => j.FirstName.Contains("Joshua"));

            // #14: All Last Name "Cooper" Players except "Joshua Cooper"
            ViewBag.NotJC = context.Players.Where(NotJC => NotJC.LastName.Contains("Cooper") &&!NotJC.FirstName.Contains("Joshua"));// or =! (without using Contains()) 
            // #15: All Players with First Name "Alexander" or "Wyatt"
            ViewBag.AlexOrWyatt = context.Players.Where(AorW => AorW.FirstName.Contains("Alexander") || AorW.FirstName.Contains("Wyatt"));

 
            // ViewBag.

            return View();

        }

        [HttpGet("level_2")]
        public IActionResult Level2()
        {
            // #1: All Teams in "Atlantic Soccer Conference"
            ViewBag.AllTeamsInAtlantic = context.Teams.Include(team => team.CurrLeague).Where(t => t.CurrLeague.Name.Contains("Atlantic Soccer Conference")).ToList();

            // #2: All Current Players on "Penguins" of Boston
            ViewBag.CurrPlayersOnBoston = context.Players.Include(p => p.CurrentTeam).Where(p => p.CurrentTeam.TeamName.Contains("Penguins")).ToList();

            // #3: All Teams in "International Collegiate Baseball Conference"
            ViewBag.TeamsInInterLeague = context.Teams.Include(t => t.CurrLeague).Where(t => t.CurrLeague.Name.Contains("International Collegiate"));

            // #4: All Teams in "American Conference of Amateur Football"
            ViewBag.TeamsInAmLeague = context.Teams.Include(t => t.CurrLeague).Where(t => t.CurrLeague.Name.Contains("American Conference of Amateur "));

            // #5: All Football Teams
            ViewBag.AllFootTeam = context.Teams.Include(t => t.CurrLeague).Where(t => t.CurrLeague.Sport.Contains("Football"));

            // #6: All Teams with a Player "Sophia"
            ViewBag.TeamsWithSophia = context.Players.Include(p => p.CurrentTeam).Where(p => p.FirstName.Contains("Sophia") || p.LastName.Contains("Sophia")).ToList();

            // #7: All Players with LastName "Flores" who doesn't currently play for the Raptors
            ViewBag.FloresNotForRaptor = context.Players.Include(p => p.CurrentTeam).Where(p => p.LastName.Contains("Flores") ); //with the query we are filtering Raptors out in View// or add this part to filter out Raptors:  p.CurrentTeam.TeamName != ...
            // && !p.CurrentTeam.TeamName.Contains("Raptors")

            // #8: All current players with the "Manitoba Tiger-Cats"
            ViewBag.CurrPlayersManitoba = context.Players.Include(p => p.CurrentTeam).Where(p => p.CurrentTeam.TeamName.Contains("Tiger-Cats") || p.CurrentTeam.Location.Contains("Manitoba"));

            //#9: teams that have had 12 or more players
            ViewBag.MoreThan12 = context.Teams.Include(t => t.AllPlayers).Where(team => team.AllPlayers.Count >= 12);
            return View();
        }

        [HttpGet("level_3")]
        public IActionResult Level3()
        {
//1. all teams, past and present Alexander Bailey has played with,

    ViewBag.AlexTeams = context.Players.Include(p => p.CurrentTeam).Include(p => p.AllTeams).ThenInclude(pt => pt.TeamOfPlayer).Where(p => p.FirstName == "Alexander" && p.LastName == "Bailey").ToList();

    //use Include in chain as above is more efficient when using ViewModel

    // ViewBag.CurrAlexTeams = context.Players.Include(p => p.CurrentTeam).Where(p => p.FirstName == "Alexander" && p.LastName == "Bailey").ToList();

// 2.all players, past and present, with the Tiger-Cats in Manitoba
    ViewBag.ManitobaAllPlayers = context.Teams.Include(team => team.CurrentPlayers).Include(t => t.AllPlayers).ThenInclude(t => t.PlayerOnTeam).Where(t => t.TeamName.Contains("Tiger-Cats") && t.Location == "Manitoba").ToList();

    // ViewBag.ManitobaCurrPlayers = context.Teams.Include(team => team.CurrentPlayers).Where(team => team.TeamName.Contains("Tiger-Cats") && team.Location == "Manitoba").ToList();

// 3. all players who were formerly (but aren't currently) with the Wichita Vikings
    ViewBag.WichitaNotCurrPlayers = context.Teams.Include(team => team.AllPlayers).ThenInclude(playerTeam => playerTeam.PlayerOnTeam).Where(team => team.TeamName.Contains("Vikings") && team.Location == "Wichita").ToList();

// 4...every team that Emily Sanchez played for before she joined the Indiana Royals
    ViewBag.EmilyTeamB4Indiana = context.Players.Include(player => player.AllTeams).ThenInclude(playerTeam => playerTeam.TeamOfPlayer).Where(player => player.FirstName == "Emily" && player.LastName == "Sanchez").ToList();

//5 ...everyone named "Levi" who has ever played in the Atlantic Federation of Amateur Baseball Players
    ViewBag.AtlanticLevi = context.Leagues.
    Include(league => league.Teams)
    .Where(league => league.Name
    .Contains("Atlantic Federation of Amateur Baseball Players")).ToList();



// ...all players, sorted by the number of teams they've played for
    ViewBag.allPlayers = context.Players
    .Include(player => player.AllTeams).ThenInclude(player => player.TeamOfPlayer)
    .OrderBy(p => p.AllTeams.Count)
    .ToList();
            return View();
        }

    }
}