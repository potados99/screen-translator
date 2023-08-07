using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Media.Ocr;

namespace ScreenTranslator.Lib.OCR
{
    internal class WindowsOcrResultProcessor
    {
        /// <summary>
        /// <c>Windows.Media.Ocr</c>의 인식 결과에 문단 구분을 추가한 <c>OcrProcessedResult</c>를 만들어 반환합니다.
        /// </summary>
        /// <param name="result">아직 처리되지 않은 <c>Windows.Media.Ocr</c>의 인식 결과</param>
        /// <returns>문단이 구분된 <c>OcrProcessedResult</c> 결과</returns>
        public OcrProcessedResult Process(OcrResult result)
        {
            return new OcrProcessedResult
            {
                Paragraphs = GroupLines(result.Lines).Select(BuildParagraph).ToList()
            };
        }

        /// <summary>
        /// 주어진 <c>OcrLine</c>들을 하나의 <c>OcrProcessedParagraph</c>로 묶어냅니다.
        /// </summary>
        /// <param name="lines">의미상으로 이어지는 하나의 문단으로 간주할 줄들입니다.</param>
        /// <returns>완성된 하나의 문단</returns>
        private OcrProcessedParagraph BuildParagraph(IEnumerable<OcrLine> lines)
        {
            return new OcrProcessedParagraph
            {
                Lines = lines.Select(l => new OcrProcessedLine
                {
                    Text = l.Text,
                    Words = l.Words.Select(w => new OcrProcessedWord
                    {
                        Text = w.Text,
                        BoundingRect = w.BoundingRect.ToSystemDrawingRect()
                    }).ToList(),
                    BoundingRect = l.Words.Select(w => w.BoundingRect.ToSystemDrawingRect()).ContainingRect()
                }).ToList(),
                BoundingRect = lines.SelectMany(l => l.Words.Select(w => w.BoundingRect.ToSystemDrawingRect()))
                    .ContainingRect()
            };
        }

        /// <summary>
        /// 주어진 <c>OcrLine</c>들 속에서 줄들의 위치를 토대로 문단을 발견합니다.
        /// 인접 노드끼리 이어진 섬을 찾는 문제와 비슷합니다.
        /// 탐색에는 DFS 알고리즘을 사용합니다.
        /// </summary>
        /// <param name="lines">모든 줄들</param>
        /// <returns>같은 문단에 속한 줄들의 컬렉션</returns>
        private IEnumerable<IEnumerable<OcrLine>> GroupLines(IEnumerable<OcrLine> lines)
        {
            var notVisitedYet = lines.ToList();
            var groups = new List<List<OcrLine>>();

            while (notVisitedYet.Any())
            {
                var visited = new List<OcrLine>();

                FindGroupRecursively(notVisitedYet.First(), notVisitedYet, visited);

                notVisitedYet.RemoveAll(l => visited.Contains(l)); // 찾아낸 줄들은 전체 목록에서 제거해줍니다.
                groups.Add(visited);
            }

            return groups;
        }

        /// <summary>
        /// 인접한 <c>OcrLine</c>으로 DFS 탐색을 전개합니다.
        /// 이 메소드는 재귀적으로 호출됩니다.
        /// </summary>
        /// <param name="current">현재 방문하고자 하는 줄입니다.</param>
        /// <param name="whole">인접한 줄을 찾을 때에 사용하는, 탐색할 줄들의 후보입니다.</param>
        /// <param name="visited">현재까지 방문한 줄들입니다.</param>
        private void FindGroupRecursively(OcrLine current, IEnumerable<OcrLine> whole, ICollection<OcrLine> visited)
        {
            if (visited.Contains(current))
            {
                // 방문한 줄이라면 더 이상 진행하지 않고 여기서 멈춥니다.
                return;
            }

            // 방문한 적이 없었던 경우라면 방문한 것으로 기록합니다.
            visited.Add(current);

            // 다음에 어떤 줄에 방문해야 하는지 알아냅니다.
            var nearbyLines = whole.Where(l =>
                    !visited.Contains(l) /*방문한 적이 없고*/
                    && Math.Abs(l.ContainingRect().Left - current.ContainingRect().Left) < 5 /*가로 시작 위치가 거의 비슷하며*/
                    && Math.Abs(l.ContainingRect().Top - current.ContainingRect().Top) < 50 /*위아래로도 인접한 줄들을 고릅니다.*/
            );

            foreach (var line in nearbyLines)
            {
                // 방문할 줄들에 대해 재귀적으로 탐색을 진행합니다.
                FindGroupRecursively(line, whole, visited);
            }
        }
    }
}