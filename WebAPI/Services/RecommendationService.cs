using Core.Entities;
using BepopStreamProject.Models.ML;
using Microsoft.ML;
using Microsoft.ML.Trainers;

namespace BepopStreamProject.Services
{
    public class RecommendationService
    {
        private readonly MLContext _mlContext;
        private ITransformer? _model;
        private PredictionEngine<SongRating, SongPrediction>? _predictionEngine;

        public RecommendationService()
        {
            _mlContext = new MLContext();
        }

        public void TrainModel(IEnumerable<PlayHistory> historyData)
        {
            if (_model != null) return;

            var data = historyData.Select(h => new SongRating
            {
                UserId = h.UserId,
                SongId = h.SongId,
                Label = 1f
            });

            var trainingData = _mlContext.Data.LoadFromEnumerable(data);

            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey(
                                outputColumnName: "userIdEncoded",
                                inputColumnName: nameof(SongRating.UserId))
                .Append(_mlContext.Transforms.Conversion.MapValueToKey(
                                outputColumnName: "songIdEncoded",
                                inputColumnName: nameof(SongRating.SongId)))
                .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(
                    labelColumnName: nameof(SongRating.Label),
                    matrixColumnIndexColumnName: "userIdEncoded",
                    matrixRowIndexColumnName: "songIdEncoded",
                    numberOfIterations: 20,
                    approximationRank: 100));

            _model = pipeline.Fit(trainingData);

            _predictionEngine = _mlContext.Model
                .CreatePredictionEngine<SongRating, SongPrediction>(_model);
        }

        public float PredictScore(int userId, int songId)
        {
            if (_predictionEngine == null)
                throw new InvalidOperationException("Model henüz eğitilmedi. Önce TrainModel çağırın.");

            var prediction = _predictionEngine.Predict(new SongRating
            {
                UserId = userId,
                SongId = songId
            });

            return prediction.Score;
        }

        public List<int> RecommendSongsForUser(int userId, List<int> allSongIds, List<int> userPlayedSongIds, int topN = 5)
        {
            var candidateSongs = allSongIds.Except(userPlayedSongIds);

            var scoredSongs = candidateSongs
                .Select(songId => new
                {
                    SongId = songId,
                    Score = PredictScore(userId, songId)
                })
                .OrderByDescending(s => s.Score)
                .Take(topN)
                .Select(s => s.SongId)
                .ToList();

            return scoredSongs;
        }
    }
}