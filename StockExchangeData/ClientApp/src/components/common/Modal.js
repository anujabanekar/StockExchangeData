﻿import React from "react";
import "./style.css";
const Modal = ({ handleClose, show, children }) => {
    const showHideClassName = show ? "modal d-block" : "modal d-none";

    return (
        <div className={showHideClassName}>
            <div className="modal-container">
                {children}
                <a href="" className="modal-close" onClick={handleClose}>
                    close
                </a>
            </div>
        </div>
    );
};

export default Modal;