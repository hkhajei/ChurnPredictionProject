from flask import Flask, request, jsonify
import joblib
import pandas as pd

app = Flask(__name__)

# Load the trained model
model = joblib.load("churn_model.joblib")

@app.route('/predict', methods=['POST'])
def predict():
    # Get data from request
    data = request.json
    # Convert data into DataFrame
    df = pd.DataFrame([data])

    # Preprocess if needed (e.g., convert categorical fields)
    df = pd.get_dummies(df, drop_first=True)

    # Make prediction
    churn_prob = model.predict_proba(df)[:, 1]  # Probability of churn

    return jsonify({"churn_probability": churn_prob[0]})

if __name__ == "__main__":
    app.run(port=5000)
