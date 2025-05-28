from Model import Model

class ModelStorage:
    def __init__(self):
        self.models: dict[str, Model] = {}

    def GetModel(self, user_id: str) -> Model:
        if (user_id not in self.models):
            self.models[user_id] = Model()
        return self.models[user_id]
    
def DeleteUserData(self, user_id: str):
    modelStorage[user_id].Clear()