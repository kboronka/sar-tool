/* Copyright (C) 2021 Kevin Boronka
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

using System.Text.RegularExpressions;

namespace sar.Tools
{
	/// <summary>
	/// Description of SearchResultMatch.
	/// </summary>
	public class SearchResultMatch
	{
		public int LineNumbrer { get; private set; }
		public Match Match { get; private set; }
		public string Reason { get; private set; }

		public SearchResultMatch(Match match, int lineNumber, string reason)
		{
			this.Match = match;
			this.LineNumbrer = lineNumber;
			this.Reason = reason;
		}

		public SearchResultMatch(Match match, int lineNumber)
			: this(match, lineNumber, "")
		{

		}
	}
}
