import pandas as pd
from sklearn.model_selection import train_test_split
from sklearn.ensemble import RandomForestClassifier
from sklearn.metrics import accuracy_score
import joblib

# Load your data (ensure you have features such as Age, TenureMonths, MonthlySpending, etc.)
data = pd.read_csv("customer_data.csv")
X = data[['Age', 'Gender_Male', 'MonthlySpending', 'TenureMonths']]
y = data['IsChurned']

# Convert categorical features to dummy variables if necessary
X = pd.get_dummies(X, drop_first=True)

# Train-test split
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

# Model training
model = RandomForestClassifier()
model.fit(X_train, y_train)

# Model evaluation
predictions = model.predict(X_test)
print(f"Accuracy: {accuracy_score(y_test, predictions)}")

# Save the model
joblib.dump(model, 'churn_model.joblib')
