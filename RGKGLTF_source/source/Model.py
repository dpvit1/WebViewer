class Model:
    def __init__(self):
        self.bodies = []

    def AddBody(self, body):
        self.bodies.append(body)
    
    def GetBodies(self) -> list:
        return self.bodies
    
    def Clear(self):
        self.bodies = []