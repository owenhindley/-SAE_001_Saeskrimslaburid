# me - this DAT
# scriptOp - the OP which is cooking

# press 'Setup Parameters' in the OP to call this function to re-create the parameters.
def onSetupParameters(scriptOp):
    page = scriptOp.appendCustomPage('Custom')
    p = page.appendFloat('Valuea', label='Value A')
    p = page.appendFloat('Valueb', label='Value B')
    return

# called whenever custom pulse parameter is pushed
def onPulse(par):
    return

def onCook(scriptOp):
    scriptOp.clear()
    
    currentState = scriptOp.inputs[0]
    velocity = scriptOp.inputs[1]
    previousState = scriptOp.inputs[2]
    
    scriptOp.copy(currentState)
    
    for s in range(0, currentState.numSamples-1):        
        # check the previous state had this many samples
        if s < previousState.numSamples:            
            # check we're updating the correct particle
            currentId = currentState['id'][s]
            prevIndex = s
            #while prevIndex >= 0 or previousState['id'][prevIndex] == currentId:
            #    prevIndex = prevIndex - 1
            
            # sample previous position
            posX = previousState['pX'][prevIndex]
            posY = previousState['pY'][prevIndex]
            
            # sample velocity
            velX = velocity['vX'][s]
            velY = velocity['vY'][s]
            
            scriptOp['pX'][s] = posX + velX
            scriptOp['pY'][s] = posY + velY
    

    return
