import React, { Component } from "react";
import "./Error.css";
class Unauthorized extends Component {
  
  render() {
    return (
      <div className="errorBody">
        <section id="not-found">
          <div id="title"></div>
          <div class="circles">
            <p style={{ color: "black" }}>
              401
              <br />
              <small>
                UNAUTHORIZED PAGE
                <br />
                <span style={{ wordBreak: "break-all" }}>
                  You are not allowed to access this page !
                </span>
              </small>
            </p>
            <span class="circle big"></span>
            <span class="circle med"></span>
            <span class="circle small"></span>
          </div>
        </section>
      </div>
    );
  }
}
export default Unauthorized;
