
function initNinthPlanet(canvasId, textureAtlasId) {
    let textureAtlas = document.getElementById(textureAtlasId);

    let canvas = document.getElementById(canvasId);
    let context = canvas.getContext('2d');

    textureAtlas.onload = function () {
        context.lineWidth = 0;
        context.fillStyle = '#ff0000';
        context.clearRect(0, 0, canvas.width, canvas.height);
        context.drawImage(textureAtlas, 1, 1, 88, 138, 25, 25, 87, 137);
    }
}