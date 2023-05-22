using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discordBot.ExternalClasses {
	internal class CardBuilder {    // can be left internal since it is only accessed by our card game commands. internal process
		private int[] cardNumbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, };
		private string[] cardSuits = { "spades", "clovers", "hearts", "diamonds" };

		// variables that can be accessed EXTERNALLY
		public int selectedNumber { get; internal set; }	// use internal set bc we don't want external code to be able to modify this variable
		public string selectedCard { get; internal set; }	

		// set up constructor
		public CardBuilder() { 
			var Random = new Random();
			int indexNumbers = Random.Next(0, this.cardNumbers.Length - 1);
			int indexSuit = Random.Next(0, this.cardSuits.Length - 1);

			this.selectedNumber = cardNumbers[indexNumbers];
			this.selectedCard = ($"{(selectedNumber != 1 ? cardNumbers[indexNumbers] : "Ace")} of {cardSuits[indexSuit]}");
		}
	}
}
