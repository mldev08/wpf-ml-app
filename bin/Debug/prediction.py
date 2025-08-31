import sys
import pandas as pd
import joblib

def main():
    if len(sys.argv) < 3:
        print("Использование: prediction.py <путь к файлу CSV> <название модели>")
        return

    input_file = sys.argv[1]
    model_name = sys.argv[2]
    model_path = rf"C:\saved_models\{model_name}.pkl"

    try:
        data = pd.read_csv(input_file)
    except Exception as e:
        print(f"Ошибка загрузки файла: {e}")
        return

    data = data.drop(columns=['current_strength'])

    try:
        loaded_model = joblib.load(model_path)
    except Exception as e:
        print(f"Ошибка загрузки модели: {e}")
        return

    try:
        predictions = loaded_model.predict(data)
        print("Предсказания:")
        for pred in predictions:
            print(pred)
    except Exception as e:
        print(f"Ошибка выполнения предсказания: {e}")

if __name__ == "__main__":
    main()
