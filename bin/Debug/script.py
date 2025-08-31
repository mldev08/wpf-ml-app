import os
import sys
import joblib
import pandas as pd
from sklearn.model_selection import train_test_split
from sklearn.linear_model import LinearRegression
from sklearn.metrics import mean_squared_error, r2_score

def training(data, model_name):
    X = data.drop("current_strength", axis=1)
    y = data["current_strength"]

    X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

    model = LinearRegression()
    model.fit(X_train, y_train)

    y_pred = model.predict(X_test)
    mse = mean_squared_error(y_test, y_pred)
    r2 = r2_score(y_test, y_pred)

    output_folder = "C:\saved_models"
    os.makedirs(output_folder, exist_ok=True)
    model_path = os.path.join(output_folder, f"{model_name}.pkl")
    
    joblib.dump(model, model_path)

    return mse, r2

def main():
    if len(sys.argv) < 3:
        print("Недостаточно аргументов. Использование: script.py <путь к файлу> <имя модели>")
        return

    file_path = sys.argv[1]
    model_name = sys.argv[2]

    try:
        # Читаем CSV файл
        df = pd.read_csv(file_path)
        print(f"Файл успешно прочитан! Количество строк: {len(df)}")
        print("------------------------------------")
        print("Корреляция:")
        print(df.corr(numeric_only=True))
    except Exception as e:
        print(f"Ошибка при чтении файла: {e}")
        return

    try:
        mse, r2 = training(df, model_name)
        print(f"Модель сохранена как {model_name}.pkl")
        print(f"Среднеквадратичная ошибка: {mse}")
        print(f"Коэффициент детерминации R^2: {r2}")
    except Exception as e:
        print(f"Ошибка при обучении модели: {e}")

if __name__ == "__main__":
    main()
