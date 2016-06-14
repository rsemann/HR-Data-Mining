var fill = d3.scale.category20();

$.ajax(
    {
        url: "/ArvoreGerada/RetornaNuvemPalavras",
        type: 'GET',
        cache: false,
        dataType: 'json',
        data: { id: $("#ArvoreGerada").val() },
        success: function (result) {
            d3.layout.cloud().size([2100, 1300])
      .words(result)
      .padding(5)
      .rotate(function () { return 0; })
      .font("Impact")
      .fontSize(function (d) { return d.sizeLetter; })
      .on("end", draw)
      .start();
        },
        error: function (error) {
        }
    })

function draw(words) {
    d3.select("#nuvem").append("svg")
        .attr("width", 2100)
        .attr("height", 1300)
        .style("padding-top", 150)
        .style("padding-left", 500)
      .append("g")
        .attr("transform", "translate(480,300)")
      .selectAll("text")
        .data(words)
      .enter().append("text")
        .style("font-size", function (d) { return d.sizeLetter + "px"; })
        .style("font-family", "Impact")
        .style("fill", function (d, i) { return fill(i); })
        .attr("text-anchor", "middle")
        .attr("transform", function (d) {
            return "translate(" + [d.x, d.y] + ")rotate(" + d.rotate + ")";
        })
        .text(function (d) { return d.text; }).append("title").text(function (d) { return d.amount; });
}