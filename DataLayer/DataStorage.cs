using Backgammon.Logic;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Backgammon.DataLayer
{
    class DataStorage
    {
        private String savefile;

        public DataStorage(String saveFile)
        {
            this.savefile = saveFile;
        }

        // Checks if saved game exists
        public bool savedGameExist()
        {
            if (File.Exists(savefile))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Saves all data
        public void saveData(GameBoard gameBoard)
        {

            Player player1 = gameBoard.player1;
            Player player2 = gameBoard.player2;
            Player activePlayer = gameBoard.activePlayer;
            DiceCup diceCup = gameBoard.diceCup;

            XDocument docSave = new XDocument(
                new XElement("SavedGame")
                );

            // Saves players
            docSave.Root.Add(
                    new XElement("Players",
                        new XElement("Player",
                            new XElement("Name", player1.name),
                            new XElement("color", player1.color),
                            new XAttribute("id", player1.id),
                            new XAttribute("active", (activePlayer.Equals(player1) ? true : false))
                        ),
                        new XElement("Player",
                            new XElement("Name", player2.name),
                            new XElement("color", player2.color),
                            new XAttribute("id", player2.id),
                            new XAttribute("active", (activePlayer.Equals(player2) ? true : false))
                        )

                    )
                );
            // Saves Dice Cup
            docSave.Root.Add(
                new XElement("diceCup",
                    from move in diceCup.getMoves()
                    select new XElement("dice", move)
                )
            );
            // Saves fields and checkers
            docSave.Root.Add(
                new XElement("gameBoard",
                    new XElement("boardField",
                        from field in gameBoard.boardFields
                        select new XElement("field",
                                new XElement("checkers",
                                    from checker in field.getCheckers()
                                    select new XElement("checker",
                                            new XElement("Player", checker.player.id), new XAttribute("id", checker.checkerId)
                                    )
                                    
                                ), new XAttribute("position", field.getPosition())
                        )
                        
                    ),
                    new XElement("eliminationField",
                        new XElement("checkers",
                            from checker in gameBoard.eliminatedField.getCheckers()
                            select new XElement("checker",
                                    new XElement("Player", checker.player.id), new XAttribute("id", checker.checkerId)
                            )
                        ), new XAttribute("position", gameBoard.eliminatedField.getPosition())
                    ),
                    new XElement("goalFields",
                        new XElement("goalField",
                            new XElement("checkers",
                                    from checker in gameBoard.goalFieldP1.getCheckers()
                                    select new XElement("checker",
                                            new XElement("Player", checker.player.id), new XAttribute("id", checker.checkerId)
                                    )

                                ), new XAttribute("owner", gameBoard.goalFieldP1.getOwner().id)
                                , new XAttribute("position", gameBoard.goalFieldP1.getPosition())
                        ),

                        new XElement("goalField",
                            new XElement("checkers",
                                    from checker in gameBoard.goalFieldP2.getCheckers()
                                    select new XElement("checker",
                                            new XElement("Player", checker.player.id), new XAttribute("id", checker.checkerId)
                                    )

                                ), new XAttribute("owner", gameBoard.goalFieldP2.getOwner().id)
                                , new XAttribute("position", gameBoard.goalFieldP2.getPosition())
                        )
                    )
                )
            );
            docSave.Save(savefile);
        }

        public XDocument getSaveFile()
        {
            XDocument doc;
            try
            {
                doc = XDocument.Load(savefile);
            }
            catch (FileNotFoundException e)
            {
                throw new NoFileException("Ops! No Such file was found");
            }
            return doc;
        }

        // Returns the players name
        public String getPlayerName(String id)
        {
            XDocument doc = getSaveFile();
            var players = from p in doc.Descendants("Players").Descendants("Player")
                          .Where(x => x.Attribute("id").Value == id)
                          select new
                          {
                              name = p.Element("Name").Value,
                          };
            String name="";
            foreach (var p in players)
            {
                name = p.name;
            }
            return name;
        }

        // Returns the player color
        public String getPlayerColor(String id)
        {
            XDocument doc = getSaveFile();
            var players = from p in doc.Descendants("Players").Descendants("Player")
                          .Where(x => x.Attribute("id").Value == id)
                          select new
                          {
                              color = p.Element("color").Value,
                          };
            String color = "";
            foreach (var p in players)
            {
                color = p.color;
            }
            return color;
        }

        // Returns the Id of acive player
        public String getActviePlayer()
        {
            XDocument doc = getSaveFile();
            var players = from p in doc.Descendants("Players").Descendants("Player")
                          .Where(x => x.Attribute("active").Value == "true")
                          select new
                          {
                              id = p.Attribute("id").Value,
                          };
            String returnId = "";
            foreach (var p in players)
            {
                returnId = p.id;
            }
            return returnId;
        }

        // Returns an array with all moves in dice cup
        public int[] getDiceCupMoves()
        {

            XDocument doc = getSaveFile();
            int[] returnMoves = null;
            if (doc.Descendants("diceCup").Descendants("dice").Any()) { 
                var moves = from m in doc.Descendants("diceCup").Descendants("dice")
                            select new
                            {
                                move = m.Value,
                            };
                returnMoves = new int[moves.Count()];
                int counter = 0;
                foreach(var m in moves)
                {
                    returnMoves[counter] = int.Parse(m.move);
                    counter++;
                }
            }
            return returnMoves;
        }

        // Returns an array with number of checker in each field position
        public int[] getCheckerPosition(String id)
        {
            XDocument doc = getSaveFile();
            int[] checkersInPositions = new int[28];

            for (int i = 0; i < checkersInPositions.Length; i++){
                checkersInPositions[i] = 0;
            }

            // Load board fields
            var fields = from f in doc.Descendants("gameBoard").Descendants("boardField").Descendants("field")
                         select new
                         {
                             position = f.Attribute("position").Value,
                         };
            foreach (var f in fields)
            {
                int position = int.Parse(f.position);

                // Load number of checkers in field that belongs to set player
                var checkers = from c in doc.Descendants("gameBoard").Descendants("boardField").Descendants("field")
                               .Where(x => x.Attribute("position").Value == f.position).Descendants("checkers").Descendants("checker")
                               .Where(x => x.Element("Player").Value == id)
                               select new
                               {
                                   id = c.Attribute("id").Value,
                               };
                checkersInPositions[position] = checkers.Count();
            }

            // Load elimination field and number of checkers for set player
            var eliminatedCheckers = from c in doc.Descendants("gameBoard").Descendants("eliminationField").Descendants("checkers").Descendants("checker")
                                     .Where(x => x.Element("Player").Value == id)
                                     select new
                                     {
                                         id = c.Attribute("id").Value,
                                         pos = c.Parent.Parent.Attribute("position").Value, // The position of the elimination field
                                     };
            int pos = 0;
            foreach (var c in eliminatedCheckers)
            {
                pos = int.Parse(c.pos);
                checkersInPositions[pos] = eliminatedCheckers.Count();
                break;
            }
            
            

            // Load goal fields and number of checkers for set player
            var goalCheckers = from c in doc.Descendants("gameBoard").Descendants("goalFields").Descendants("goalField")
                                .Where(x => x.Attribute("owner").Value == id).Descendants("checkers").Descendants("checker")
                                select new
                                {
                                    id = c.Attribute("id").Value,
                                    pos = c.Parent.Parent.Attribute("position").Value, // The position of the goal field
                                };

            foreach (var c in goalCheckers)
            {
                pos = int.Parse(c.pos);
                checkersInPositions[pos] = goalCheckers.Count();
                break;
            }
            

            return checkersInPositions;
        }

    }
}
