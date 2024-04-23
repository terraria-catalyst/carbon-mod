import {createPublicizer} from "publicizer";

export const publicizer = createPublicizer("Carbon");

publicizer.createAssembly("tModLoader").publicizeAll();