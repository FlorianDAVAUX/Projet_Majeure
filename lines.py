import cv2
import numpy as np
import matplotlib.pyplot as plt

# Lecture de l'image
In = cv2.imread('images_test/diag1.jpg')
I = cv2.cvtColor(In, cv2.COLOR_RGB2GRAY)

# Creation de l'image des contours
I_contour = cv2.Canny(I, 50, 150)  # 50 et 150 sont les seuils min et max
lignes, colonnes = I_contour.shape

# #################### TEST 1 : TAILLE IMAGE DES CONTOURS #########################
# print('lignes:', lignes, 'colonnes:', colonnes)

# #################### TEST 2 : CONTOUR #########################
# plt.figure(0)
# plt.title('Image des contours')
# plt.imshow(I_contour, cmap='gray')
# plt.show()

# Definition des parametres de l'algorithme
rhomax = np.sqrt((lignes - 1)**2 + (colonnes - 1)**2)  # Distance max entre l'origine et un point de l'image, cad la diagonale de l'image
drho = 1
rhoL = round(2 * rhomax / drho) + 1   # Nombre de valeurs de rho

thetamax = np.pi                      # Angle max entre la normale et l'axe horizontal (la normale étant perpendiculaire à la droite détectée)
dtheta = np.pi / 180                  # Pas de theta (1 degre en radians)
thetaL = round(thetamax / dtheta)     # Nombre de valeurs de theta

########################### TEST 3 : PARAMETRES #########################
# print('rhomax:', rhomax, 'drho:', drho, 'rhoL:', rhoL)
# print('thetamax:', thetamax, 'dtheta:', dtheta, 'thetaL:', thetaL)

# Calcul de la matrice d'accumulation
H = np.zeros((rhoL, thetaL))

for i in range(lignes):
    for j in range(colonnes):
        if I_contour[i, j] == 255:
            for o in range(thetaL):
                t = o * dtheta                       # Angle theta en radians (nombre de degres * pas de theta)
                p = i * np.cos(t) + j * np.sin(t)    # Distance rho 
                H[int((p + rhomax) / drho), o] += 1  # Incrementation de la valeur de H à la position (p, t)

#################### TEST 4 : ACCUMULATEUR #########################
# plt.figure(1)
# plt.imshow(H, aspect='auto', cmap='viridis')
# plt.colorbar()
# plt.title('Matrice d\'accumulation')
# plt.xlabel('theta')
# plt.ylabel('rho')
# plt.show()

# Extraction des maximas locaux
Hmax = H.copy()

seuil = 0
n = 2

vmax = np.zeros((n, 2))

for i in range(n):
    maxval = np.max(Hmax)
    if maxval < seuil:
        break
    maxind = np.argmax(Hmax)
    row, col = np.unravel_index(maxind, H.shape)
    vmax[i, :] = [row, col]
    Hmax[row, col] = 0

#################### TEST 5 : MAXIMAS LOCAUX #########################
# print('vmax:', vmax)

plt.figure(1)
plt.title('Image initiale avec les droites détectées')

# Calcul des points extremaux de chaque droite
angles = []

for k in range(n):
    # on récupère les angles
    t_ind = int(vmax[k, 1])
    td = t_ind * dtheta
    angles.append(td)

# droite moyenne des n droites
t = np.mean(vmax[:, 1]) * dtheta
p = np.mean(vmax[:, 0]) * drho - rhomax

# Identifions les pixels des contours proches de cette droite
selected_points = []
for i in range(lignes):
    for j in range(colonnes):
        if I_contour[i, j] == 255:  # Si le pixel appartient au contour
            # Vérifie si le pixel (i, j) est sur la droite
            distance = abs(i * np.cos(t) + j * np.sin(t) - p)
            if distance < 1:  # Tolérance pour compenser les imprécisions
                selected_points.append((i, j))  # Stocke les coordonnées (x, y)

#################### TEST 6 : POINTS SELECTIONNES #########################
# selected_points_print = np.array(selected_points)
# plt.scatter(selected_points_print[:, 1], selected_points_print[:, 0], c='r', s=1)

# Si des points sont détectés sur la droite
if selected_points:
    selected_points = np.array(selected_points)
    y_vals = selected_points[:, 0]  # Coordonnées x
    x_vals = selected_points[:, 1]  # Coordonnées y

    # Trouver les points extrêmes
    X1, X2 = x_vals.min(), x_vals.max()
    Y1, Y2 = y_vals.min(), y_vals.max()

    #################### TEST 7 : POINTS EXTREMAUX #########################
    # plt.scatter(X1, Y1, c='b', s=10)
    # plt.scatter(X2, Y2, c='b', s=10)

    # on attribue les valeur en fonction de l'orientation de la droite
    # si la droite est orienté entre 0 et 90 degrés
    if t >= 0 and t < np.pi / 2:
        X = [X2, Y1]
        Y = [X1, Y2]
    # si la droite est orienté entre 90 et 180 degrés
    elif t >= np.pi / 2 and t <= np.pi:
        X = [X1, Y1]
        Y = [X2, Y2]

    # Affichage des droites
    plt.plot([X[0], Y[0]], [X[1], Y[1]], 'r')



# Affichage des images
plt.imshow(In, cmap='gray')
plt.show()

# Calcul de l'angle moyen
a = np.mean(angles) * 180 / np.pi
# if a >= 85 and a <= 95:
#     # La résistance est orientée verticalement
#     return True
# else:
#     # Aucune ligne verticale détectée, la résistance n'est pas orientée verticalement
#     return False
